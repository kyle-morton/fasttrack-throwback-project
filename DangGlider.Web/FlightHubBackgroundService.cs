using AutoMapper;
using DangGlider.Web.Core.Data;
using DangGlider.Web.Core.Domain;
using DangGlider.Web.Core.Dto;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.Web
{

    public interface IFlightHub
    {
        Task OnCreate(FlightDto flight);
        Task OnDeparture(int flightId);
        Task OnArrival(int flightId);
        Task OnTimeUpdate(DateTime currentTime);
    }

    public class FlightHubBackgroundService : IFlightHub, IHostedService
    {

        private readonly ILogger<FlightHubBackgroundService> _logger;
        private readonly IServiceProvider _services;
        private HubConnection _connection;

        public FlightHubBackgroundService(IServiceProvider services, ILogger<FlightHubBackgroundService> logger, IMapper mapper)
        {
            _logger = logger;
            _services = services;

            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7007/hubs/flight")
                .Build();

            _connection.On<int>("OnArrival", OnArrival);
            _connection.On<int>("OnDeparture", OnDeparture);
            _connection.On<FlightDto>("OnCreate", OnCreate);
            _connection.On<DateTime>("OnTimeUpdate", OnTimeUpdate);
        }

        private IServiceScope getScope()
        {
            return _services.CreateScope();
        }

        private DangGliderDbContext getContext(IServiceScope scope)
        {
            return scope.ServiceProvider.GetRequiredService<DangGliderDbContext>();
        }

        private IMapper getMapper(IServiceScope scope)
        {
            return scope.ServiceProvider.GetRequiredService<IMapper>();
        }

        public async Task OnArrival(int flightId)
        {
            _logger.LogInformation("Arrived: {flightId}", flightId);
            var scope = getScope();
            var context = getContext(scope);

            var flight = await context.Flights.SingleOrDefaultAsync(f => f.FlightNumber == flightId);

            if (flight == null)
            {
                return;
            }

            flight.HasArrived = true;
            await context.SaveChangesAsync();

            scope.Dispose();
        }

        public async Task OnCreate(FlightDto flightDto)
        {
            _logger.LogInformation("Created: {flightId}", flightDto.Id);

            var scope = getScope();
            var context = getContext(scope);
            var mapper = getMapper(scope);

            var originGeoCode = await context.GeoCodes.FirstOrDefaultAsync(g => g.City == flightDto.Origin.City && g.State == flightDto.Origin.State);
            var destGeoCode = await context.GeoCodes.FirstOrDefaultAsync(g => g.City == flightDto.Destination.City && g.State == flightDto.Destination.State);

            if (originGeoCode == null || destGeoCode == null)
            {
                scope.Dispose();
                return;
            }

            var flight = mapper.Map<Flight>(flightDto);
            flight.FlightNumber = flight.Id;
            flight.Id = 0;
            flight.Origin = flight.Destination = null;
            flight.OriginId = originGeoCode.Id;
            flight.DestinationId = destGeoCode.Id;

            await context.Flights.AddAsync(flight);
            await context.SaveChangesAsync();

            scope.Dispose();
        }

        public async Task OnDeparture(int flightId)
        {
            _logger.LogInformation("Departed: {flightId}", flightId);

            var scope = getScope();
            var context = getContext(scope);

            var flight = await context.Flights.SingleOrDefaultAsync(f => f.FlightNumber == flightId);

            if (flight == null)
            {
                scope.Dispose();
                return;
            }

            flight.HasDeparted = true;
            await context.SaveChangesAsync();
            scope.Dispose();
        }

        public Task OnTimeUpdate(DateTime currentTime)
        {
            _logger.LogInformation("OnTimeUpdate: {time}", currentTime.ToString("hh:mm tt"));

            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Loop is here to wait until the server is running
            while (true)
            {
                try
                {
                    await _connection.StartAsync(cancellationToken);

                    break;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _connection.DisposeAsync();
        }
    }
}
