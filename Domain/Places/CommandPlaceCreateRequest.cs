using termoservis.api.Domain.Abstract;

namespace termoservis.api.Domain.Places
{
    public class CommandPlaceCreateRequest : ICommandRequest
    {
        public string Name { get; set; }

        public long CountryId { get; set; }
    }
}