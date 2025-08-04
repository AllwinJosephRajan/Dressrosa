namespace Dressrosa.Model
{
    public class AppSettings
    {
        public string RefreshTokenExpiryMinutes { get; set; }
        public string IDTokenExpiryMinutes { get; set; }
        public string Secret { get; set; }
        public string ExpireTime { get; set; }
        public string Audience { get; set; }
    }
}
