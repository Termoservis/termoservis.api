using Microsoft.AspNetCore.Mvc;

namespace termoservis.api.Controllers
{
    public static class ApiError 
    {
        public static ObjectResult BadRequest(this Controller controller, string code, string message) 
        {
            return controller.BadRequest(new Error {
                ErrorCode = code,
                ErrorMessage = message
            });
        }

        public class Error 
        {
            public string ErrorCode { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}