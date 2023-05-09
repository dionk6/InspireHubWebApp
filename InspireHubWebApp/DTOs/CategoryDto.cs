using System.ComponentModel.DataAnnotations;

namespace InspireHubWebApp.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        public string Title { get; set; }
        public int OrderNo { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
