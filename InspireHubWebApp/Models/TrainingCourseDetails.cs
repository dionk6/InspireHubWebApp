namespace InspireHubWebApp.Models
{
    public class TrainingCourseDetails
    {
        public int Id { get; set; }
        public int TrainingId { get; set; }
        public int CourseDetailId { get; set; }
        public Training Training { get; set; }
        public CourseDetail CourseDetail { get; set; }
    }
}
