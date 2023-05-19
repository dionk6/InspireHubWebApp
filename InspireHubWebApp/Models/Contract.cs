using System.ComponentModel.DataAnnotations;

namespace InspireHubWebApp.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string? ContractDate { get; set; }
        [Required(ErrorMessage = "First Name is Required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is Required")]
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PersonalNumber { get; set; }
        public string? Price { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
