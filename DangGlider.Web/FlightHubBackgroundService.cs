using DangGlider.Web.Dto;
using Microsoft.AspNetCore.SignalR.Client;

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
        private HubConnection _connection;

        public FlightHubBackgroundService(ILogger<FlightHubBackgroundService> logger)
        {
            _logger = logger;

            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7007/hubs/flight")
                .Build();

            _connection.On<int>("OnArrival", OnArrival);
            _connection.On<int>("OnDeparture", OnDeparture);
            _connection.On<FlightDto>("OnCreate", OnCreate);
            _connection.On<DateTime>("OnTimeUpdate", OnTimeUpdate);
        }

        public Task OnArrival(int flightId)
        {
            _logger.LogInformation("Arrived: {flightId}", flightId);

            return Task.CompletedTask;
        }

        public Task OnCreate(FlightDto flight)
        {
            _logger.LogInformation("Created: {flightId}", flight.Id);

            return Task.CompletedTask;
        }

        public Task OnDeparture(int flightId)
        {
            _logger.LogInformation("Departed: {flightId}", flightId);

            return Task.CompletedTask;
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
