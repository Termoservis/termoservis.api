using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using termoservis.api.Data;
using termoservis.api.Domain.Abstract;

namespace termoservis.api.Domain.Places
{
    public class QueryPlacesFiltered : IQueryHandlerAsync<QueryPlacesFilteredRequest, QueryPlacesFilteredResponse>
    {
        private readonly IPlacesRepository placesRepository;


        public QueryPlacesFiltered(IPlacesRepository placesRepository)
        {
            this.placesRepository = placesRepository ?? throw new System.ArgumentNullException(nameof(placesRepository));
        }


        public async Task<QueryPlacesFilteredResponse> HandleAsync(QueryPlacesFilteredRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            var keywords = request.Keywords
                .ToLowerInvariant()
                .Split(new[] { " ", ".", "-" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var query = this.placesRepository.QueryAll();
            if (keywords.Count > 0)
                query = query.Where(place => keywords.Contains(place.Name.ToLower()));
            query = query.OrderBy(place => place.Name);

            var placesList = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync(cancellationToken);

            return new QueryPlacesFilteredResponse(placesList);
        }
    }
}