using AutoMapper;
using DangGlider.FlightGen.API.Dto;
using DangGlider.FlightGen.API.Hubs;
using DangGlider.FlightGen.Core.Data;
using DangGlider.FlightGen.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.FlightGen.API
{
    public class FlightBackgroundService : BackgroundService
    {
        private readonly ILogger<FlightBackgroundService> _logger;
        private readonly IHubContext<FlightHub, IFlightHub> _flightHub;
        private DateTime _currentTime;

        public FlightBackgroundService(IServiceProvider services, ILogger<FlightBackgroundService> logger, IHubContext<FlightHub, IFlightHub> flightHub)
        {
            Services = services;
            _logger = logger;
            _flightHub = flightHub;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service running.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service is working.");

            _currentTime = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = Services.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IFlightService>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                    await _flightHub.Clients.All.OnTimeUpdate(_currentTime);

                    if (_currentTime.Minute.IsBetween(0, 30))
                    {
                        await DoCreate(service, mapper, stoppingToken);
                    }
                    await DoUpdate(service, mapper, stoppingToken);

                    await Task.Delay(2500, stoppingToken);

                    _currentTime = _currentTime.AddMinutes(30);
                }
            }

        }

        private async Task DoCreate(IFlightService service, IMapper mapper, CancellationToken stoppingToken)
        {
            var newFlight = await service.CreateRandomAsync(_currentTime, stoppingToken);

            _logger.LogInformation("New Flight: " + newFlight.Id + " " + newFlight.Origin.City + " - to - " + newFlight.Destination.City);
            _logger.LogInformation("Departure: " + newFlight.ScheduledDeparture.ToString("MM/dd/yy hh:mm tt") + " - Arrival: " + newFlight.ScheduledArrival.ToString("MM/dd/yy hh:mm tt"));

            var dto = mapper.Map<FlightDto>(newFlight);

            await _flightHub.Clients.All.OnCreate(dto);
        }

        private async Task DoUpdate(IFlightService service, IMapper mapper, CancellationToken stoppingToken)
        {
            var departedFlights = await service.UpdateDepartedFlightsAsync(_currentTime, stoppingToken);
            foreach(var flightId in departedFlights)
            {
                await _flightHub.Clients.All.OnDeparture(flightId);
            }

            var arrivedFlights = await service.UpdateArrivedFlightsAsync(_currentTime, stoppingToken);
            foreach(var flightId in arrivedFlights)
            {
                await _flightHub.Clients.All.OnArrival(flightId);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }

    }

    public static class FlightServiceExtensions
    {
        public static bool IsBetween(this int number, int start, int end)
        {
            return number >= start && number <= end;
        }
    }

}
