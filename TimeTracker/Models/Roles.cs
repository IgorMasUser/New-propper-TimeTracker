﻿using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Models
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        public int UserRoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string Description { get; set; }

        public bool isActive { get; set; }

        public DateTime RolesCreatedAt { get; set; } = DateTime.UtcNow;
    }
}