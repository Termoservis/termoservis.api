using System;
using System.Threading;
using System.Threading.Tasks;
using termoservis.api.Data;
using termoservis.api.Domain.Abstract;
using termoservis.api.Models;

namespace termoservis.api.Domain.Countries
{
    public class CommandCountryCreate : ICommandHandlerAsync<CommandCountryCreateRequest, CommandCountryCreateResponse>
    {
        private readonly ICountriesRepository countriesRepository;


        public CommandCountryCreate(ICountriesRepository countriesRepository)
        {
            this.countriesRepository = countriesRepository ?? throw new ArgumentNullException(nameof(countriesRepository));
        }


        public async Task<CommandCountryCreateResponse> HandleAsync(
            CommandCountryCreateRequest request, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (cancellationToken == null) throw new ArgumentNullException(nameof(cancellationToken));

            var response = new CommandCountryCreateResponse();
            try 
            {
                var countryId = await this.countriesRepository
                    .AddAsync(new Country {
                        Name = request.Name.Trim(),
                        ShortName = request.ShortName.Trim()
                    });

                response.IsSuccess = true;
                response.CountryId = countryId;
            }
            catch(Exception ex)
            {
                response.Error = ex;
            }
            return response;
        }
    }
}