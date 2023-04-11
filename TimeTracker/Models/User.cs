using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace TimeTracker.Models
{
    public class User
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; } = new byte[32];

        public byte[] PasswordSalt { get; set; } = new byte[32];

        public bool IsSystemAdmin { get; set; } = false;

        public int Role { get; set; }

        public float Salary { get; set; }

        public string ApprovalStatus { get; set; }

        public Guid ApprovalId { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartedWorkDayAt { get; set; } = (DateTime)SqlDateTime.MinValue;

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FinishedWorkDayAt { get; set; } = (DateTime)SqlDateTime.MinValue;

        public int Break { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; } = (DateTime)SqlDateTime.MinValue;

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TotalWorkedPerDay { get; set; } = (DateTime)SqlDateTime.MinValue;

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime UserWorkedPerRequestedPeriod { get; set; } = (DateTime)SqlDateTime.MinValue;

        [NotMapped]
        public int Numeration { get; set; }
    }
}
