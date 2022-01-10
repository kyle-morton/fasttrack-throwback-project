using DangGlider.Web.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.Web.Core.Data
{
    public class DangGliderDbContext : DbContext
    {
        public DangGliderDbContext(DbContextOptions<DangGliderDbContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<GeoCode> GeoCodes { get; set; }

    }
}
