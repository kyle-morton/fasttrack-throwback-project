using AutoMapper;
using DangGlider.Web.Core.Data;
using DangGlider.Web.Core.Domain;
using DangGlider.Web.Core.Dto;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.Web.Core.Services
{
    public interface IFlightService
    {
        Task<FlightDto> CreateAsync(FlightDto flightDto);
        Task UpdateArrivedAsync(int flightId);
        Task UpdateDepartedAsync(int flightId);
    }

    public class FlightService : IFlightService
    {
        private DangGliderDbContext _context { get; }
        private IMapper _mapper { get; }

        public FlightService(DangGliderDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FlightDto> CreateAsync(FlightDto flightDto)
        {
            var originGeoCode = await _context.GeoCodes.FirstOrDefaultAsync(g => g.City == flightDto.Origin.City && g.State == flightDto.Origin.State);
            var destGeoCode = await _context.GeoCodes.FirstOrDefaultAsync(g => g.City == flightDto.Destination.City && g.State == flightDto.Destination.State);

            if (originGeoCode == null || destGeoCode == null)
            {
                return null;
            }

            var flight = _mapper.Map<Flight>(flightDto);
            flight.FlightNumber = flight.Id;
            flight.Id = 0;
            flight.Origin = null;
            flight.Destination = null;
            flight.OriginId = originGeoCode.Id;
            flight.DestinationId = destGeoCode.Id;

            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();

            return _mapper.Map<FlightDto>(flight);  
        }

        public async Task UpdateDepartedAsync(int flightId)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
            if (flight == null)
            {
                return;
            }

            flight.HasDeparted = true;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateArrivedAsync(int flightId)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
            if (flight == null)
            {
                return;
            }

            flight.HasArrived = true;
            await _context.SaveChangesAsync();
        }
    }
}
