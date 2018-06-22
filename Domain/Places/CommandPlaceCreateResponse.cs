using termoservis.api.Domain.Abstract;

namespace termoservis.api.Domain.Places
{
    public class CommandPlaceCreateResponse : CommandResponse
    {
        public long PlaceId { get; set; }
    }
}