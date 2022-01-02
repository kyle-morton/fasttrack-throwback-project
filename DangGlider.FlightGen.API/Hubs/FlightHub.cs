using DangGlider.FlightGen.Core.Domain;
using Microsoft.AspNetCore.SignalR;

namespace DangGlider.FlightGen.API.Hubs
{

    public interface IFlightHub
    {
        Task OnCreate(Flight flight);
        //Task OnUpdate(Flight flight);
    }

    public class FlightHub : Hub<IFlightHub>
    {
        public async Task SendOnCreate(Flight flight)
        {
            await Clients.All.OnCreate(flight);
        }
    }
}
