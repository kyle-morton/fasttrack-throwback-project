using DangGlider.FlightGen.Core.Domain;

namespace DangGlider.FlightGen.Core.Services
{

    public interface IFlightService
    {
        Task CreateAsync(CancellationToken cancellationToken);
        Task UpdateAsync(Flight flight, CancellationToken cancellationToken);
    }

    public class FlightService : IFlightService
    {
        public FlightService()
        {

        }

        public Task CreateAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Flight flight, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
