using DangGlider.FlightGen.Core.Data;
using DangGlider.FlightGen.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.FlightGen.API
{
    public class FlightBackgroundService : BackgroundService
    {
        private readonly ILogger<FlightBackgroundService> _logger;
        private DateTime _currentTime;

        public FlightBackgroundService(IServiceProvider services, ILogger<FlightBackgroundService> logger)
        {
            Services = services;
            _logger = logger;
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

                    var newFlight = await service.CreateRandomAsync(_currentTime, stoppingToken);

                    _logger.LogInformation("New Flight: " + newFlight.Id + " " + newFlight.Origin.City + " - to - " + newFlight.Destination.City);
                    _logger.LogInformation("Departure: " + newFlight.ScheduledDeparture.ToString("MM/dd/yy hh:mm tt") + " - Arrival: " + newFlight.ScheduledArrival.ToString("MM/dd/yy hh:mm tt"));

                    await Task.Delay(5000, stoppingToken);

                    _currentTime = _currentTime.AddMinutes(30);
                }
            }

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }

}
