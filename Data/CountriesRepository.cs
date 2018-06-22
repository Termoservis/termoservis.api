using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ITermoservisContext context;

        public CountriesRepository(ITermoservisContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<long> AddAsync(Country country, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (country == null) throw new ArgumentNullException(nameof(country));
            if (cancellationToken == null) throw new ArgumentNullException(nameof(cancellationToken));

            this.context.Countries.Add(country);
            await this.context.SaveChangesAsync(cancellationToken);

            return country.Id;   
        }

        public IQueryable<Country> QueryAll()
        {
            return this.context.Countries;
        }
    }
}