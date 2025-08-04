using Dressrosa.Dto;

namespace Dressrosa.Service
{
    public interface IUserService
    {
        Task<(bool IsRestricted, TokenResponseDto TokenResponse)> Auth(TokenRequestDto tokenRequestDto, string timeZone);
        Task<TokenResponseDto> RefreshToken(TokenRequestDto tokenRequestDto, string timeZone);
        Task<UserDto> AddUserAsync(UserDto user);
        Task<UserDto> GetUserByIdAsync(string userId);
    }
}
