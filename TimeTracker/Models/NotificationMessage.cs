using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Models
{
    public class NotificationMessage
    {
        [Required]
        public string Id { get; set; } = $"message:{Guid.NewGuid().ToString()}";

        [Required]
        public string Message { get; set; } = String.Empty;
    }
}
