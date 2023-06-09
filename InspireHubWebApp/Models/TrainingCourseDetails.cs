﻿namespace InspireHubWebApp.Models
{
    public class TrainingCourseDetails
    {
        public int Id { get; set; }
        public int TrainingId { get; set; }
        public int CourseDetailId { get; set; }
        public string Description { get; set; }
        public Training Training { get; set; }
        public CourseDetail CourseDetail { get; set; }
        public int OrderNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
