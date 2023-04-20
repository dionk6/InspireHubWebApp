using System.ComponentModel.DataAnnotations;

namespace InspireHubWebApp.DTOs
{
    public class Application
    {
        public string? CourseTitle { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string? Phone { get; set; }
        public string? Message { get; set; }
    }
}
