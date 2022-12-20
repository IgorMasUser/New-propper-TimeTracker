using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TimeTracker.Models
{
    public class User
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "Out of acceptable range")]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public bool IsSystemAdmin { get; set; }

        public int Role { get; set; }

        public Guid? UserIdentityId { get; set; }
        [Required]

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Started { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Finished { get; set; }

        [Required]
        [Range(1, 59, ErrorMessage = "Out of acceptable range")]
        public int Break { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TotalWorked { get; set; }

        [NotMapped]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime UserTotalWorked { get; set; }

        [NotMapped]
        public int Numeration { get; set; }

    }
}
