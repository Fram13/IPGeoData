using System.Linq;

namespace IPGeoData.Model
{
    public interface IRepository<T>
    {
        IQueryable<T> All { get; }

        void Add(T item);
        void Update(T item);        
    }
}
