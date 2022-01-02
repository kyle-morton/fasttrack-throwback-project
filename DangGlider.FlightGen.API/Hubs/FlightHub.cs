using DangGlider.FlightGen.API.Dto;
using Microsoft.AspNetCore.SignalR;

namespace DangGlider.FlightGen.API.Hubs
{

    public interface IFlightHub
    {
        Task OnCreate(FlightDto flight);
        //Task OnUpdate(Flight flight);
    }

    public class FlightHub : Hub<IFlightHub>
    {
        public async Task SendOnCreate(FlightDto flight)
        {
            await Clients.All.OnCreate(flight);
        }
    }
}
