using termoservis.api.Domain.Abstract;

namespace termoservis.api.Domain.Countries
{
    public class CommandCountryCreateRequest : ICommandRequest
    {
        public string Name { get; set; }

        public string ShortName { get; set; }
    }
}