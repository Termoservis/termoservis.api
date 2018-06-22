using System.Collections.Generic;
using System.Threading.Tasks;
using termoservis.api.Data;

namespace termoservis.api.Services
{
    public interface IPlacesApiService
    {
        Task<long> CreatePlaceAsync(
            string name,
            long countryId
        );

        Task<IEnumerable<PlaceDto>> QueryPlacesByKeywordsAsync(
            string keywords,
            int skip,
            int take
        );
    }
}