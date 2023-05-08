namespace InspireHubWebApp.Models
{
    public class CourseDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderNo { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
