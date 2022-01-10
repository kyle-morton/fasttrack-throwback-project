using DangGlider.Web.Core.Data;

namespace DangGlider.Web.Core.Services
{
    public interface IFlightService
    {

    }

    public class FlightService : IFlightService
    {
        private DangGliderDbContext _context { get; }

        public FlightService(DangGliderDbContext context)
        {
            _context = context;
        }

        

    }
}
