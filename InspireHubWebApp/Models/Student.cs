﻿namespace InspireHubWebApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public int TrainingId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public Training Training { get; set; }
    }
}
