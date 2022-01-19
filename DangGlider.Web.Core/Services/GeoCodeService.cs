using DangGlider.Web.Core.Data;

namespace DangGlider.Web.Core.Services
{
    public interface IGeoCodeService
    {

    }

    public class GeoCodeService : IGeoCodeService
    {
        private DangGliderDbContext _context { get; }
        public GeoCodeService(DangGliderDbContext context)
        {
            _context = context;
        }
    }
}
