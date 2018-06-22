using System;
using System.Threading.Tasks;
using termoservis.api.Domain.Abstract;
using termoservis.api.Domain.Countries;

namespace termoservis.api.Services
{
    public class CountriesApiService : ICountriesApiService
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;

        public CountriesApiService(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            this.queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        }


        public async Task<long> CreateCountryAsync(string name, string shortName)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (string.IsNullOrWhiteSpace(shortName)) throw new ArgumentException(nameof(shortName));

            var response = await this.commandDispatcher.DispatchAsync<CommandCountryCreate, CommandCountryCreateRequest, CommandCountryCreateResponse>(
                new CommandCountryCreateRequest {
                    Name = name,
                    ShortName = shortName
                }
            );
            if (!response.IsSuccess)
                throw response.Error;

            return response.CountryId;
        }
    }
}