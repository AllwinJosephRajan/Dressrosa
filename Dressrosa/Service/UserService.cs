using Dressrosa.Converter;
using Dressrosa.Core.Extensions;
using Dressrosa.Data;
using Dressrosa.Dto;
using Dressrosa.Model;
using Dressrosa.Services;
using Dressrosa.Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Dressrosa.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConverter<UserDto, User> _userConverter;
        private readonly AppSettings _appSettings;
        private readonly PasswordHasher _hasher;
        private readonly IConverter<TokenRequestDto, TokenRequest> _tokenConverter;
        public UserService(IUserRepository userRepository, IConverter<UserDto, User> userConverter, PasswordHasher hasher, IConverter<TokenRequestDto, TokenRequest> tokenConverter,
            IOptions<AppSettings> appsettings)
        {
            _userRepository = userRepository;
            _tokenConverter = tokenConverter;
            _userConverter = userConverter;
            _appSettings = appsettings.Value;
            _hasher = hasher;
        }
        public async Task<(bool IsRestricted, TokenResponseDto TokenResponse)> Auth(TokenRequestDto tokenRequestDto, string timeZone)
        {
            try
            {
                var entity = _tokenConverter.Convert(tokenRequestDto);
                User user;

                // Determine if input is an email or a username
                if (IsValidEmail(entity.EmailAddressOrUserName))
                {
                    user = await _userRepository.GetUserByEmailAsync(entity.EmailAddressOrUserName);
                }
                else
                {
                    user = await _userRepository.GetUserByUserNameAsync(entity.EmailAddressOrUserName);
                }

                if (user == null || !VerifyPassword(entity.Password, user.Password))
                {
                    return (false, null);
                }
                // Check if user has valid RoleId = '1'
                if (user.UserRoleMapping == null || !user.UserRoleMapping.Any(r => r.RoleId == "1"))
                {
                    return (true, null); // User is restricted due to invalid role
                }
                // Generate tokens
                var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
                var tokenHandler = new JwtSecurityTokenHandler();
                var newRefreshToken = CreateRefreshToken(timeZone);
                var encodedToken = await CreateAccessToken(user, entity, newRefreshToken, timeZone);


                return (false, encodedToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool IsValidEmail(string input)
        {
            try
            {
                var email = new System.Net.Mail.MailAddress(input);
                return email.Address == input;
            }
            catch
            {
                return false;
            }
        }
        private bool VerifyPassword(string plainTextPassword, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedInput = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword)));
                return hashedInput == hashedPassword;
            }
        }
        private Token CreateRefreshToken(string timeZone)
        {
            try
            {
                double tokenExpiryTime = Convert.ToDouble(_appSettings.RefreshTokenExpiryMinutes);
                return new Token()
                {
                    Value = Guid.NewGuid().ToString("N"),
                    CreatedDate = DateTime.UtcNow.ConvertUTCToLocalTimeBasedOnTimeZone(timeZone),
                    ExpiryTime = DateTime.UtcNow.ConvertUTCToLocalTimeBasedOnTimeZone(timeZone).AddMinutes(tokenExpiryTime)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private async Task<TokenResponseDto> CreateAccessToken(User result, TokenRequest tokenRequest, Token newRefreshToken, string timeZone)
        {
            try
            {
                // authentication successful so generate jwt token
                double tokenExpiryTime = Convert.ToDouble(_appSettings.IDTokenExpiryMinutes);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                  new Claim(ClaimTypes.Name,tokenRequest.EmailAddressOrUserName),
           new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim("UserId",result.Id),
                    }),

                    //Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTime),
                    Expires = DateTime.UtcNow.ConvertUTCToLocalTimeBasedOnTimeZone(timeZone).AddMinutes(tokenExpiryTime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var newtoken = tokenHandler.CreateToken(tokenDescriptor);
                var encodedToken = tokenHandler.WriteToken(newtoken);
                return new TokenResponseDto
                {
                    Token = encodedToken,
                    RefreshToken_Expiration = newRefreshToken.ExpiryTime,
                    Expiration = newtoken.ValidTo.ConvertUTCToLocalTimeBasedOnTimeZone(timeZone),
                    Refresh_Token = newRefreshToken.Value,
                    Username = tokenRequest.EmailAddressOrUserName,
                    UserId = result.Id
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<TokenResponseDto> RefreshToken(TokenRequestDto tokenRequestDto, string timeZone)
        {
            try
            {
                var entity = _tokenConverter.Convert(tokenRequestDto);

                entity.Password = HashPassword(entity.Password);

                var user = await _userRepository.Login(entity);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid username or password.");
                }

                // Ensure user has valid RoleId = '1'
                if (user.UserRoleMapping == null || !user.UserRoleMapping.Any(r => r.RoleId == "1"))
                {
                    throw new UnauthorizedAccessException("User is not authorized to access this system.");
                }

                var newRefreshToken = CreateRefreshToken(timeZone);
                var encodedToken = await CreateAccessToken(user, entity, newRefreshToken, timeZone);

                return encodedToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        private string HashPassword(string plainTextPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword)));
            }
        }



        public async Task<UserDto> AddUserAsync(UserDto user)
        {
            try
            {
                // Check if the email address already exists in the database
                bool emailExists = await _userRepository.CheckEmailExistsAsync(user.EmailAddress);

                if (emailExists)
                {
                    throw new ValidationException("Email address is already in use.");
                }

                // Check if the username already exists
                bool userNameExists = await _userRepository.CheckUserNameExistsAsync(user.UserName);
                if (userNameExists)
                {
                    throw new ValidationException("Username is already taken.");
                }
                // Hash the password
                if (!string.IsNullOrEmpty(user.Password))
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                        user.Password = Convert.ToBase64String(hashedBytes);
                    }
                }
                else
                {
                    throw new ValidationException("Password cannot be null or empty.");
                }

                // Convert the UserDto to the data model
                var newUser = _userConverter.Convert(user);

                // Add the new user to the repository
                var addedUser = await _userRepository.AddUserAsync(newUser);

                if (addedUser != null)
                {
                    return _userConverter.Convert(addedUser);
                }
                else
                {
                    throw new InvalidOperationException("User could not be added to the database.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            try
            {
                // Retrieve timezone from the request
                //string timeZoneId = httpContext.Request.Headers["TimeZone"];
                //TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                var users = await _userRepository.GetUserByIdAsync(userId);


                if (users == null)
                {
                    return null;
                }

                var userDto = _userConverter.Convert(users);
                //userDto.UpdateOn = ConvertToTimeZone(users.UpdateOn, timeZone);
                //userDto.UpdateOn = ConvertToTimeZone(users.UpdateOn, timeZone);
                return userDto;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing user data.", ex);
            }

        }
        private static DateTime? ConvertToTimeZone(DateTime? dateTime, TimeZoneInfo targetTimeZone)
        {
            if (!dateTime.HasValue) return null;

            // Assume database time is in UTC, if not, first convert it to UTC
            DateTime utcTime = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);

            // Convert to the target time zone
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, targetTimeZone);
        }
    }
}
