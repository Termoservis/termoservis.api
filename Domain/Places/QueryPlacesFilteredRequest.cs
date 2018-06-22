using termoservis.api.Domain.Abstract;

namespace termoservis.api.Domain.Places
{
    public class QueryPlacesFilteredRequest : IQueryRequest
    {
        public string Keywords { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}