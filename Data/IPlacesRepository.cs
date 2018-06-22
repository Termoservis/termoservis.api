using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public interface IPlacesRepository
    {
        Task<long> AddAsync(Place place, CancellationToken cancellationToken = default(CancellationToken));
        
        IQueryable<Place> QueryAll();
    }
}