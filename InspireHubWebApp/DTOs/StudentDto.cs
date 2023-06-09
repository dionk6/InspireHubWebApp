﻿namespace InspireHubWebApp.DTOs
{
    public class StudentListDto
    {
        public int TrainingId { get; set; }
        public List<StudentDto> Students { get; set; }
    }
    public class StudentDto
    {
        public int Id { get; set; }
        public int TrainingId { get; set; }
        public string FirstName{ get; set; }
        public string LastName{ get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Training { get; set; }
        public decimal Price { get; set; }
        public string DateApplied { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }
        public bool IsPaid { get; set; }
        public bool HasInvoice { get; set; }
    }
}
