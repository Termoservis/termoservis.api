using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using termoservis.api.Data;

namespace termoservis.api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesApiService countriesApiService;

        public CountriesController(ICountriesApiService countriesApiService)
        {
            this.countriesApiService = countriesApiService ?? throw new System.ArgumentNullException(nameof(countriesApiService));
        }


        [HttpPost]
        public async Task<IActionResult> Create(string name, string shortName)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(shortName))
            {
                return BadRequest();
            }

            var result = await this.countriesApiService.CreateCountryAsync(name, shortName);
            return Ok(result);
        }
    }
}