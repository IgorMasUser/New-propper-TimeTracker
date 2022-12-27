using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Models
{
    public class Roles
    {
        public int UserRoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool isActive { get; set; }

        public DateTime RolesCreatedAt { get; set; } = DateTime.UtcNow;
    }
}