using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public static class RequestExtensions
    {
        private const string ErrorMessage = "Unable to parse token from header";

        /// <summary>
        /// Extract timezome from header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetTimeZone(this HttpRequest request)
        {
            try
            {
                var timeZone = request.Headers["TimeZone"];
                if (!string.IsNullOrEmpty(timeZone))
                {
                    return timeZone;
                }

                throw new UnauthorizedAccessException(ErrorMessage);
            }
            catch
            {
                throw new UnauthorizedAccessException(ErrorMessage);
            }
        }


        public static string GetUserId(this HttpRequest request)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var authHeader = request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader))
                {
                    throw new UnauthorizedAccessException("Authorization header is missing.");
                }

                var token = authHeader.Replace("Bearer ", "");
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var userIdClaim = tokenS?.Claims.FirstOrDefault(claim => claim.Type == "UserId")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    return userIdClaim;
                }

                throw new UnauthorizedAccessException("UserId claim is missing in the token.");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Failed to extract UserId from token: {ex.Message}");
            }
        }
    }
}
