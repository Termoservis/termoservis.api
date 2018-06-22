using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public interface ICountriesRepository 
    {
        Task<long> AddAsync(Country country, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<Country> QueryAll();
    }
}