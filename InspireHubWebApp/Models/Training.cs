namespace InspireHubWebApp.Models
{
    public class Training
    {
        public int Id { get; set; }
        public int Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public string ApplicationDeadline { get; set; }
        public string Instructor { get; set; }
        public string InstructorBio { get; set; }
        public string InstructorImage { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string BehanceUrl { get; set; }
        public string DribbbleUrl { get; set; }
        public string LinkedinUrl { get; set; }
    }
}
