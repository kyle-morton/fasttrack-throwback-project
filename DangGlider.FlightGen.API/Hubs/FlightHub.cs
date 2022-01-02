using DangGlider.FlightGen.API.Dto;
using Microsoft.AspNetCore.SignalR;

namespace DangGlider.FlightGen.API.Hubs
{

    public interface IFlightHub
    {
        Task OnCreate(FlightDto flight);
        Task OnDeparture(int flightId);
        Task OnArrival(int flightId);
        Task OnTimeUpdate(DateTime currentTime);
    }

    public class FlightHub : Hub<IFlightHub>
    {
    }
}
