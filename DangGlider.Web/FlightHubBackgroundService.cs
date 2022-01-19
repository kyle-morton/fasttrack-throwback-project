using AutoMapper;
using DangGlider.Web.Core.Dto;
using DangGlider.Web.Core.Services;
using DangGlider.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace DangGlider.Web
{

    public interface IFlightReceiverHub
    {
        Task OnCreate(FlightDto flight);
        Task OnDeparture(int flightId);
        Task OnArrival(int flightId);
        Task OnTimeUpdate(DateTime currentTime);
    }

    public class FlightHubBackgroundService : IFlightReceiverHub, IHostedService
    {

        private readonly ILogger<FlightHubBackgroundService> _logger;
        private readonly IServiceProvider _services;
        private HubConnection _providerHubConnection;
        private readonly IHubContext<FlightHub, IFlightHub> _flightHub;

        public FlightHubBackgroundService(IConfiguration config, IServiceProvider services, ILogger<FlightHubBackgroundService> logger, IHubContext<FlightHub, IFlightHub> flightHub)
        {
            _logger = logger;
            _services = services;
            _flightHub = flightHub;

            _providerHubConnection = new HubConnectionBuilder()
                .WithUrl(config["FlightProvider"])
                .Build();

            _providerHubConnection.On<int>("OnArrival", OnArrival);
            _providerHubConnection.On<int>("OnDeparture", OnDeparture);
            _providerHubConnection.On<FlightDto>("OnCreate", OnCreate);
            _providerHubConnection.On<DateTime>("OnTimeUpdate", OnTimeUpdate);
        }

        private IServiceScope getScope()
        {
            return _services.CreateScope();
        }

        private IFlightService getFlightService(IServiceScope scope)
        {
            return scope.ServiceProvider.GetRequiredService<IFlightService>();
        }

        public async Task OnArrival(int flightId)
        {
            _logger.LogInformation("Arrived: {flightId}", flightId);
            var scope = getScope();
            var flightService = getFlightService(scope);

            await flightService.UpdateArrivedAsync(flightId);
            await _flightHub.Clients.All.OnArrival(flightId);
            scope.Dispose();

            
        }

        public async Task OnCreate(FlightDto flightDto)
        {
            _logger.LogInformation("Created: {flightId}", flightDto.Id);

            var scope = getScope();
            var flightService = getFlightService(scope);

            var flight = await flightService.CreateAsync(flightDto);
            await _flightHub.Clients.All.OnCreate(flight);
            scope.Dispose();
        }

        public async Task OnDeparture(int flightId)
        {
            _logger.LogInformation("Departed: {flightId}", flightId);

            var scope = getScope();
            var flightService = getFlightService(scope);

            await flightService.UpdateDepartedAsync(flightId);
            await _flightHub.Clients.All.OnDeparture(flightId);
            scope.Dispose();
        }

        public async Task OnTimeUpdate(DateTime currentTime)
        {
            _logger.LogInformation("OnTimeUpdate: {time}", currentTime.ToString("hh:mm tt"));
            await _flightHub.Clients.All.OnTimeUpdate(currentTime);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Loop is here to wait until the server is running
            while (true)
            {
                try
                {
                    await _providerHubConnection.StartAsync(cancellationToken);

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
            await _providerHubConnection.DisposeAsync();
        }
    }
}
