using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using termoservis.api.Data;
using termoservis.api.Services;

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
                return this.BadRequest("places-create-name-required", "argumnet name is required.");
            }
            if (!countryId.HasValue)
            {
                return this.BadRequest("places-create-countryid-required", "argument countryId is required.");
            }
            if (countryId.Value <= 0) 
            {
                return this.BadRequest("places-create-countryid-outofrange", "argument countryId is invalid. Must be larger than zero.");
            }

            var result = await this.placesApiService.CreatePlaceAsync(name, countryId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the filtered places.
        /// </summary>
        /// <param name="keywords">The filter keywords.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The number of items to take. Required. One is minimum.</param>
        /// <param name="nonce">The request nonce. Required.</param>
        /// <returns>Returns the specified nonce and collection of places.</returns>
        [HttpGet("filtered")]
        [ProducesResponseType(typeof(PlacesFilteredDto), 200)]
        [ProducesResponseType(typeof(ApiError.Error), 400)]
        public async Task<IActionResult> GetFilteredAsync(string keywords, int? skip, int? take, int? nonce)
        {
            if (!nonce.HasValue)
            {
                return this.BadRequest("nonce-required", "argument nonce is required.");
            }
            if (skip.HasValue && skip < 0)
            {
                return this.BadRequest("places-filtered-skip-outofrange", "argument skip can't be less than zero.");
            }
            if (!take.HasValue)
            {
                return this.BadRequest("places-filtered-take-required", "argument take is required.");
            }
            if (take.HasValue && take <= 0)
            {
                return this.BadRequest("places-filtered-take-outofrange", "argument take is invalid. Must be larger than zero.");
            }

            var result = new PlacesFilteredDto
            {
                Places = await this.placesApiService.QueryPlacesByKeywordsAsync(
                    keywords ?? string.Empty, 
                    skip ?? 0, 
                    take.Value),
                Nonce = nonce.Value
            };
            return Ok(result);
        }

        public class PlacesFilteredDto 
        {
            public IEnumerable<PlaceDto> Places { get; set; }

            public int Nonce { get; set; }
        }
    }
}