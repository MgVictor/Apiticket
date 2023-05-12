namespace apiTicket.Utils.Authentication
{
    using System;
    public class AccessToken
    {
        private static readonly TimeSpan Threshold = new TimeSpan(1, 0, 0);
        public AccessToken(string token, int expiresInSeconds) : this(token, null, expiresInSeconds)
        {

        }
        public AccessToken(
            string token,
            string refreshToken,
            int expiresInSeconds)
        {
            Token = token;
            RefreshToken = refreshToken;
            ExpiresInSeconds = expiresInSeconds;
            Expires = DateTime.UtcNow.AddSeconds(ExpiresInSeconds);
        }
        public string Token { get; }
        public string RefreshToken { get; }
        public int ExpiresInSeconds { get; }
        public DateTime Expires { get; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public bool Expired => (Expires - DateTime.UtcNow).TotalSeconds <= Threshold.TotalSeconds;
    }
}
