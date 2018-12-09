using IPGeoData.Model;
using IPGeoData.WebService.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace IPGeoData.WebService.Models
{
    public class IPLocationsManager
    {
        private readonly IDataContext _context;
        private readonly IGeoWebServicesClientFactory _clientFactory;
        private readonly ILogger<IPLocationsManager> _logger;

        public IPLocationsManager(IDataContext context, IGeoWebServicesClientFactory clientFactory, ILogger<IPLocationsManager> logger)
        {
            _context = context;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public Location Get(IPAddress ip)
        {
            var location = _context.IPLocations.All.Where(l => l.IP == ip).SingleOrDefault();

            if (location == null)
            {
                _logger.LogInformation("Receiving location data for IP {0}", ip);

                using (var client = _clientFactory.CreateClient())
                {
                    var info = client.City(ip);

                    location = new IPLocation
                    {
                        IP = ip,
                        Continent = info.Continent.Name,
                        Country = info.Country.Name,
                        City = info.City.Name,
                        Latitude = info.Location.Latitude,
                        Longitude = info.Location.Longitude
                    };
                }

                _context.IPLocations.Add(location);
                _context.SaveChanges();
            }

            return new Location
            {
                Continent = location.Continent,
                Country = location.Country,
                City = location.City,
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
    }
}
