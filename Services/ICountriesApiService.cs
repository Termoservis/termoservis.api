using System.Threading.Tasks;

namespace termoservis.api.Services
{
    public interface ICountriesApiService 
    {
        Task<long> CreateCountryAsync(
            string name,
            string shortName
        );
    }
}