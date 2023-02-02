
namespace TimeTracker.Models
{
    public class RefreshTokenProvider
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenCreatedAt { get; set; } = DateTime.Now;

        public DateTime RefreshTokenExpiresAt { get; set; }

    }
}

