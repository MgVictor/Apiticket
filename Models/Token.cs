using System;

namespace apiTicket.Models
{
    public class Token
    {
        private static readonly TimeSpan Threshold = new TimeSpan(1, 0, 0);
        public string TokenC { get; }
        public string RefreshToken { get; }
        public int ExpiresInSeconds { get; }
        public DateTime Expires { get; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public bool Expired => (Expires - DateTime.UtcNow).TotalSeconds <= Threshold.TotalSeconds;
    }
}
