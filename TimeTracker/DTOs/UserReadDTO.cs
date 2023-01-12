using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TimeTracker.DTOs

{
    public class UserReadDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        //public string Email { get; set; } = string.Empty;
        public string Email { get; set; } = "igor@gmail.com";

        //public string Password { get; set; } = string.Empty;
        public string Password { get; set; } = "12345";

        public bool IsSystemAdmin { get; set; } = false;

        public Guid? UserAccessTokenPair { get; set; }

        public int Role { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartedWorkDayAt { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FinishedWorkDayAt { get; set; }

        public int Break { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TotalWorkedPerDay { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime UserWorkedPerRequestedPeriod { get; set; }

        [NotMapped]
        public int Numeration { get; set; }
    }
}
