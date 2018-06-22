using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public class TermoservisContext : IdentityDbContext<ApplicationUser>, ITermoservisContext
    {
        public TermoservisContext(DbContextOptions options) : base(options)
        {
        }

        protected TermoservisContext()
        {
        }


        public DbSet<Country> Countries { get; set; }

        public DbSet<Place> Places { get; set; }
    }
}