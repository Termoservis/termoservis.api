using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using termoservis.api.Data;

namespace termoservis.api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class PlacesController : Controller
    {
        private readonly IPlacesApiService placesApiService;


        public PlacesController(IPlacesApiService placesApiService)
        {
            this.placesApiService = placesApiService ?? throw new System.ArgumentNullException(nameof(placesApiService));
        }


        [HttpPost]
        public async Task<IActionResult> Create(string name, long? countryId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("argumnet name is required.");
            }
            if (!countryId.HasValue)
            {
                return BadRequest("argument countryId is required.");
            }
            if (countryId.Value <= 0) 
            {
                return BadRequest("argument countryId is invalid. Must be larger than zero.");
            }

            var result = await this.placesApiService.CreatePlaceAsync(name, countryId.Value);
            return Ok(result);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredAsync(string keywords, int? skip, int? take, int? nonce)
        {
            if (!nonce.HasValue)
            {
                return BadRequest("argument nonce is required.");
            }
            if (skip.HasValue && skip < 0)
            {
                return BadRequest("argument skip can't be less than zero.");
            }
            if (!take.HasValue)
            {
                return BadRequest("argument take is required.");
            }
            if (take.HasValue && take <= 0)
            {
                return BadRequest("argument take is invalid. Must be larger than zero.");
            }

            var result = new
            {
                places = await this.placesApiService.QueryPlacesByKeywordsAsync(
                    keywords ?? string.Empty, 
                    skip ?? 0, 
                    take.Value),
                nonce = nonce
            };
            return Ok(result);
        }
    }
}