using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Models
{
    public class AzureIdentityProvider
    {
        public int UserId { get; set; }

        public Guid? UserIdentityId { get; set; } = new Guid();

        public Guid? AzurAuthenticationKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}

