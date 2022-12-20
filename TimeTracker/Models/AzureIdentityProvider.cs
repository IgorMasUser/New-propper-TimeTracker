using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Models
{
    public class AzureIdentityProvider
    {
        [Key]
        public int UserId { get; set; }

        public Guid AzurAuthenticationKey { get; set; }
    }
}
