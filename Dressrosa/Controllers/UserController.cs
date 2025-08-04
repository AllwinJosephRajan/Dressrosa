using Dressrosa.Controllers;
using Dressrosa.Dto;
using Dressrosa.Service;
using Dressrosa.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Safemax.API.Controllers
{
    [Route("admin/api/v1/user")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : BaseController
    {
        public readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] TokenRequestDto tokenRequestDto)
        {
            try
            {
                _logger.LogError("user logged in");
                var timeZone = HttpContext.Request.GetTimeZone();
                switch (tokenRequestDto.GrantType)
                {
                    case "password":
                        var (isRestricted, tokenResponse) = await _userService.Auth(tokenRequestDto, timeZone);

                        if (isRestricted)
                        {
                            return Ok(new ResponseDto<object>
                            {
                                Status = ResponseStatus.Error,
                                Message = "Access Denied: Your account does not have permission to log in.",
                                Data = null
                            });
                        }

                        _logger.LogInformation("Authentication successful for user.");
                        return GetOKResponse(tokenResponse);
                    case "refresh_token":
                        var refreshTokenResponse = await _userService.RefreshToken(tokenRequestDto, timeZone);
                        _logger.LogInformation("Token refresh successful for user");
                        return GetOKResponse(refreshTokenResponse);
                }
                return null;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred for user");
                return GetUnhandledErrorResponse(tokenRequestDto);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDto userDto)
        {
            try
            {
                //var userId = Request.GetUserId(); // Extract UserId from token
                //if (string.IsNullOrEmpty(userId))
                //{
                //    return Unauthorized("Invalid token. Please provide a valid token.");
                //}
                ResponseDto<UserDto> response = new()
                {
                    Data = await _userService.AddUserAsync(userDto)
                };
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access: " + ex.Message);
                return Unauthorized("Unauthorized access.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while : {ex.Message}");
                return StatusCode(500, $"An error occurred while processing your request");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
        {
            try
            {
                //var userId = Request.GetUserId(); // Extract UserId from token
                //if (string.IsNullOrEmpty(userId))
                //{
                //    return Unauthorized("Invalid token. Please provide a valid token.");
                //}
                ResponseDto<UserDto> response = new()
                {
                    Data = await _userService.GetUserByIdAsync(id)
                };
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access: " + ex.Message);
                return Unauthorized("Unauthorized access.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting User information");
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }

        }
    }
}
