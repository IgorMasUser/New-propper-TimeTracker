
namespace TimeTracker.Models
{
    public class RefreshTokenProvider
    {
        public int UserId { get; set; }

        public Guid? UserRefreshTokenPair { get; set; } = new Guid();

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenCreated { get; set; } = DateTime.Now;

        public DateTime RefreshTokenExpires { get; set; }

    }
}

