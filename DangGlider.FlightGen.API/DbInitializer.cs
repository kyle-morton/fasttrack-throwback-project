using DangGlider.FlightGen.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.FlightGen.Core.Data
{
    public static class DbInitializer
    {
        public static void Populate(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<FlightGenDbContext>(), env);
            }
        }

        private static void SeedData(FlightGenDbContext context, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                Console.WriteLine("--> Attempting to add migrations...");

                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--> Could not run migrations: " + ex.Message);
                }
            }

            if (context.GeoCodes.Any())
            {
                Console.WriteLine("--> We already have data");
                return;
            }

            Console.WriteLine("--> Seeding data...");

            context.GeoCodes.AddRange(
                new GeoCode { City = "Los Angeles", State = "CA" },
                new GeoCode { City = "Dallas", State = "TX" },
                new GeoCode { City = "Houston", State = "TX" },
                new GeoCode { City = "New York City", State = "NY" },
                new GeoCode { City = "Nashville", State = "TN" },
                new GeoCode { City = "Las Vegas", State = "NV" }
            );

            context.SaveChanges();
        }
    }

}
