using System.Linq;

namespace IPGeoData.Model
{
    public class IPLocationsRepository : IRepository<IPLocation>
    {
        private readonly DatabaseContext _context;

        public IPLocationsRepository(DatabaseContext context)
        {
            _context = context;
        }

        public IQueryable<IPLocation> All => _context.IPLocations;

        public void Add(IPLocation item)
        {
            _context.Add(item);
        }

        public void Update(IPLocation item)
        {
            _context.Update(item);
        }
    }
}
