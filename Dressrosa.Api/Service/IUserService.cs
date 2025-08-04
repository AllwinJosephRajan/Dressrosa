using Dressrosa.Api.Dto;

namespace Dressrosa.Api.Service
{
    public interface IUserService
    {
        Task<(bool IsRestricted, TokenResponseDto TokenResponse)> Auth(TokenRequestDto tokenRequestDto, string timeZone);
        Task<TokenResponseDto> RefreshToken(TokenRequestDto tokenRequestDto, string timeZone);
        Task<UserDto> AddUserAsync(UserDto user);
    }
}
