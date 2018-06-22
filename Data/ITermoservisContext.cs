using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public interface ITermoservisContext
    {
        DbSet<Place> Places { get; set; }

        DbSet<Country> Countries { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}