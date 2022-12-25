﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TimeTracker.DTOs

{
    public class UserCreateDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsSystemAdmin { get; set; } = false;

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartedWorkDayAt { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FinishedWorkDayAt { get; set; }

        public int Break { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TotalWorkedPerDay { get; set; }

    }
}
