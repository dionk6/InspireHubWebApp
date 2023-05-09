namespace InspireHubWebApp.DTOs
{
    public class TrainingDetailDto
    {
        public int Id { get; set; }
        public int? TrainingId { get; set; }
        public string Training { get; set; }
        public string CourseDetail { get; set; }
        public int CourseDetailId { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public int OrderNo { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
