using System;
using System.Collections.Generic;
using System.Text;

namespace IPGeoData.Model
{
    public class DataContext : IDataContext
    {
        private readonly DatabaseContext _dbContext;
        private IRepository<IPLocation> _ipLocations;

        public DataContext(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<IPLocation> IPLocations
        {
            get
            {
                if (_ipLocations == null)
                {
                    _ipLocations = new IPLocationsRepository(_dbContext);
                }

                return _ipLocations;
            }
        }

        public void Dispose()
        {
            SaveChanges();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
