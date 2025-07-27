namespace Dressrosa.Api.Dto
{
    public class TokenResponseDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string password { get; set; }
        public string Refresh_Token { get; set; }
        public string UserRoleIds { get; set; }
        public List<string> Permissions { get; set; }
        public DateTime RefreshToken_Expiration { get; set; }
        public DateTime Expiration { get; set; }
    }
}
