using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using IPGeoData.Model;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;

namespace IPGeoData.DatabaseUpdater
{
    internal class Application
    {
        private readonly string _archivePath = "GeoLite2-City.tar.gz";
        private readonly string _databasePathPattern = "*.mmdb";
        private readonly string _extractionDirectoryPath = "Geo2IP Database";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        [Required]
        [Option("--url <DATABASE_URL>", Description = "Geo2IP database URL.")]
        public string DatabaseURL { get; }

        public Application(IServiceProvider serviceProvider, ILogger<Application> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<Application>>();
        }

        private void DownloadFile(string url, string path)
        {
            _logger.LogInformation("Dowmloading Geo2IP database from {0} to {1}.", url, path);

            var request = WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;

            var response = request.GetResponse();

            int clusterSize = 4096;

            using (var stream = response.GetResponseStream())
            using (var file = File.Create(path))
            {
                var buffer = new byte[clusterSize];
                var outBuffer = new byte[clusterSize];

                var read = stream.Read(buffer, 0, buffer.Length);

                while (read > 0)
                {
                    byte[] temp = outBuffer;
                    outBuffer = buffer;
                    buffer = temp;

                    var writingTask = file.WriteAsync(outBuffer, 0, read);

                    read = stream.Read(buffer, 0, buffer.Length);

                    writingTask.Wait();
                }
            }

            _logger.LogInformation("Dowmload complete.");
        }

        private void ExtractTarGz(string archivePath, string destinationDirectory)
        {
            _logger.LogInformation("Extracting Geo2IP database from {0} to {1}", archivePath, destinationDirectory);

            using (var inStream = File.OpenRead(archivePath))
            using (var gzipStream = new GZipInputStream(inStream))
            using (var tarArchive = TarArchive.CreateInputTarArchive(gzipStream))
            {
                tarArchive.ExtractContents(destinationDirectory);
            }

            _logger.LogInformation("Extraction complete.");
        }

        private void UpdateDatabase(string geo2IPDbPath)
        {
            _logger.LogInformation("Updating database from {0}.", geo2IPDbPath);

            using (var context = _serviceProvider.GetRequiredService<IDataContext>())
            using (var reader = new DatabaseReader(geo2IPDbPath))
            {
                int count = context.IPLocations.All.Count();
                int batchSize = 100;
                int batchCount = count / batchSize + (count % batchSize > 0 ? 1 : 0);

                var locations = context.IPLocations.All.Take(batchCount).ToList();

                for (int i = 0; i < batchCount; i++)
                {
                    var nextLocations = context.IPLocations.All.Skip((i + 1) * batchCount).Take(batchCount).ToListAsync();

                    foreach (var location in locations)
                    {
                        try
                        {
                            var info = reader.City(location.IP);
                            location.City = info.City.Name;
                            location.Country = info.Country.Name;
                            location.Continent = info.Continent.Name;
                            location.Latitude = info.Location.Latitude;
                            location.Longitude = info.Location.Longitude;
                        }
                        catch (AddressNotFoundException exc)
                        {
                            _logger.LogError(exc, "IP: {0}", location.IP);
                        }
                    }

                    locations = nextLocations.Result;
                    context.SaveChanges();

                    _logger.LogInformation("Updated {0} of {1} batches", i + 1, batchCount);
                }
            }

            _logger.LogInformation("Update complete.");
        }

        private void OnExecute()
        {
            try
            {
                DownloadFile(DatabaseURL, _archivePath);

                var dbDir = Path.Combine(Environment.CurrentDirectory, _extractionDirectoryPath);
                ExtractTarGz(_archivePath, dbDir);

                var dbPath = Directory.EnumerateFiles(dbDir, _databasePathPattern, SearchOption.AllDirectories).FirstOrDefault();
                UpdateDatabase(dbPath);

                _logger.LogInformation("Clearing working directory.");

                Directory.Delete(dbDir, true);
                File.Delete(_archivePath);

                _logger.LogInformation("Clearing complete.");
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc.Message);
            }
        }
    }
}
