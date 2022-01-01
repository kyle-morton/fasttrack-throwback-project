using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DangGlider.Web.Data
{
    public class AppUserDbContext : IdentityDbContext
    {
        public AppUserDbContext(DbContextOptions<AppUserDbContext> options)
            : base(options)
        {
        }
    }
}