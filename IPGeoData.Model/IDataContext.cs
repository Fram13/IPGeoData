using System;

namespace IPGeoData.Model
{
    public interface IDataContext : IDisposable
    {
        IRepository<IPLocation> IPLocations { get; }

        void SaveChanges();
    }
}
