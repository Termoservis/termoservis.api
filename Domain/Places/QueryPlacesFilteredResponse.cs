using System;
using System.Collections.Generic;
using termoservis.api.Domain.Abstract;
using termoservis.api.Models;

namespace termoservis.api.Domain.Places
{
    public class QueryPlacesFilteredResponse : IQueryResponse
    {
        public QueryPlacesFilteredResponse(IEnumerable<Place> places)
        {
            this.Places = places ?? throw new ArgumentNullException(nameof(places));
        }


        public IEnumerable<Place> Places { get; }
    }
}