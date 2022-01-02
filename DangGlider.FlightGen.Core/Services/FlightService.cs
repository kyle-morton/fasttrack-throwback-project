using DangGlider.FlightGen.Core.Data;
using DangGlider.FlightGen.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace DangGlider.FlightGen.Core.Services
{

    public interface IFlightService
    {
        Task<Flight> CreateAsync(Flight flight, CancellationToken cancellationToken);
        Task<Flight> CreateRandomAsync(DateTime currentTime, CancellationToken cancellationToken);
        Task<Flight> UpdateAsync(Flight flight, CancellationToken cancellationToken);
        Task<int> GetRandomAsync(CancellationToken cancellationToken);
    }

    public class FlightService : IFlightService
    {
        private readonly FlightGenDbContext _context;

        public FlightService(FlightGenDbContext context)
        {
            _context = context;
        }

        public async Task<Flight> CreateRandomAsync(DateTime currentTime, CancellationToken cancellationToken)
        {
            var geocodeIds = await _context.GeoCodes.Select(g => g.Id).ToListAsync();
            var origin = await GetRandomGeoCode(geocodeIds);
            var destination = await GetRandomGeoCode(geocodeIds, origin.Id);

            var random = new Random();

            var scheduledDeparture = currentTime.AddHours(random.Next(2, 12));
            var scheduledArrival = scheduledDeparture.AddHours(random.Next(2, 12));

            var flight = new Flight
            {
                OriginId = origin.Id,
                DestinationId = destination.Id, 
                ScheduledDeparture = scheduledDeparture,
                ScheduledArrival = scheduledArrival
            };

            return await CreateAsync(flight, cancellationToken);
        }

        public async Task<Flight> CreateAsync(Flight flight, CancellationToken cancellationToken)
        {
            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();

            return flight;
        }

        public Task<int> GetRandomAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Flight> UpdateAsync(Flight flight, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<GeoCode> GetRandomGeoCode(List<int> geoCodeIds, int idToSkip = 0)
        {
            var random = new Random();
            var index = random.Next(geoCodeIds.Count);
            var codeId = geoCodeIds[index];
            while (codeId == idToSkip)
            {
                index = random.Next(geoCodeIds.Count);
                codeId = geoCodeIds[index];
            }

            return await _context.GeoCodes.SingleOrDefaultAsync(g => g.Id == codeId);
        }
    }

}
