using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace termoservis.api.Data
{
    public class TermoservisContext : IdentityDbContext<ApplicationUser>
    {
        public TermoservisContext(DbContextOptions options) : base(options)
        {
        }

        protected TermoservisContext()
        {
        }
    }
}