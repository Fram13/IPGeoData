using Microsoft.EntityFrameworkCore;

namespace IPGeoData.Model
{
    public class DatabaseContext : DbContext
    {
        public DbSet<IPLocation> IPLocations { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IPLocation>().ToTable("iplocation");
            modelBuilder.Entity<IPLocation>().Property(l => l.ID).HasColumnName("id");
            modelBuilder.Entity<IPLocation>().Property(l => l.IP).HasColumnName("ip");
            modelBuilder.Entity<IPLocation>().Property(l => l.City).HasColumnName("city");
            modelBuilder.Entity<IPLocation>().Property(l => l.Country).HasColumnName("country");
            modelBuilder.Entity<IPLocation>().Property(l => l.Continent).HasColumnName("continent");
            modelBuilder.Entity<IPLocation>().Property(l => l.Latitude).HasColumnName("latitude");
            modelBuilder.Entity<IPLocation>().Property(l => l.Longitude).HasColumnName("longitude");
        }
    }
}
