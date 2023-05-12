using System.ComponentModel.DataAnnotations;

namespace InspireHubWebApp.DTOs
{
    public class TrainingDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required(ErrorMessage = "Start Date is Required")]
        public string? StartDateString { get; set; }
        [Required(ErrorMessage = "End Date is Required")]
        public string? EndDateString { get; set; }
        [Required]
        public string Hours { get; set; }
        [Required]
        public string Days { get; set; }
        [Required(ErrorMessage = "Short Description is Required")]
        public string ShortDescription { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        [Required(ErrorMessage = "Application Deadline is Required")]
        public string ApplicationDeadline { get; set; }
        [Required]
        public string Instructor { get; set; }
        [Required(ErrorMessage = "Instructor Position is Required")]
        public string InstructorPosition { get; set; }
        [Required(ErrorMessage = "Instructor Bio is Required")]
        public string InstructorBio { get; set; }
        public string? InstructorImage { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? BehanceUrl { get; set; }
        public string? DribbbleUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public int? OrderNo { get; set; }
        public bool? Show { get; set; }
        public bool? IsDeleted { get; set; }
        public string? TrainingDates { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [Required(ErrorMessage = "Instructor Name is Required")]
        public IFormFile? InstructorPhoto{ get; set; }
        public IFormFile? InstructorUpdatePhoto{ get; set; }
    }
}
