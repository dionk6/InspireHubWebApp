using System.ComponentModel.DataAnnotations;

namespace InspireHubWebApp.DTOs
{
    public class TrainingDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Instructor { get; set; }
        public decimal Price { get; set; }
        public int OrderNo { get; set; }
        public string CreateDate { get; set; }
        [Required(ErrorMessage = "Shtoni nje foto!")]
        public IFormFile? InstructorImage { get; set; }
    }
}
