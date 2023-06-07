using System.ComponentModel.DataAnnotations;

namespace InspireHubWebApp.DTOs
{
    public class Application
    {
        public int? Id { get; set; }
        public int? TrainingId { get; set; }
        public string? CourseTitle { get; set; }
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string? Phone { get; set; }
        public string? Message { get; set; }
        [Required]
        public string token { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsViewed { get; set; }
    }
}
