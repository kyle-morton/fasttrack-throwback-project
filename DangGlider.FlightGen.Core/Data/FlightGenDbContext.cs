using DangGlider.FlightGen.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.FlightGen.Core.Data
{
    public class FlightGenDbContext : DbContext
    {

        public FlightGenDbContext(DbContextOptions<FlightGenDbContext> options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}

        public DbSet<GeoCode> GeoCodes { get; set; }
        public DbSet<Flight> Flights { get; set; }
    }
}
