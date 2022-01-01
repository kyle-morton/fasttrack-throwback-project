using DangGlider.FlightGen.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.FlightGen.API
{
    public class FlightBackgroundService : BackgroundService
    {
        private readonly ILogger<FlightBackgroundService> _logger;

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

            while(!stoppingToken.IsCancellationRequested)
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<FlightGenDbContext>();
                    var geocodes = await context.GeoCodes.ToListAsync(stoppingToken);

                    _logger.LogInformation("Geocodes found: " + geocodes.Count);

                    await Task.Delay(5000, stoppingToken);
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
