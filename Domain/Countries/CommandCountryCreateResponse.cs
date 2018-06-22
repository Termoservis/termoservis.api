using termoservis.api.Domain.Abstract;

namespace termoservis.api.Domain.Countries
{
    public class CommandCountryCreateResponse : CommandResponse
    {
        public long CountryId { get; set; }
    }
}