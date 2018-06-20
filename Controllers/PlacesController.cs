using Microsoft.AspNetCore.Mvc;

namespace termoservis.api.Controllers
{
    [ApiVersion( "1.0" )]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class PlacesController : Controller
    {
        [HttpPost]
        public IActionResult Get([FromBody]PlacesFilterDto filter)
        {
            var result = new string[] { "test", "test2" };

            return Ok(result);
        }

        public class PlacesFilterDto
        {
            public string SearchTerms { get; set; }
        }
    }
}