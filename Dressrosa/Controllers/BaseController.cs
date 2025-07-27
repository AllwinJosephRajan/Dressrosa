using Microsoft.AspNetCore.Mvc;
using Dressrosa.Enum;
using Dressrosa.Dto;

namespace Dressrosa.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult GetResponse<T>(int statusCode,
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
           T data, string message = "") where T : class
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        {
            var response = new ResponseDto<T>
            {
                Data = data,
                Message = message,
                Status = ResponseStatus.Success,

            };
            return StatusCode(statusCode, response);
        }

        public IActionResult GetOKResponse<T>(T data, string message = "") where T : class
        {
            return GetResponse(200, data, message);
        }

        public IActionResult GetUnhandledErrorResponse<T>(T data) where T : class
        {
            return GetResponse(500, data, "Oops!! Something bad has happened");
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.


    }
}
