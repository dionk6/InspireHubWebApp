﻿namespace InspireHubWebApp.DTOs
{
    public class TrainingView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Dates { get; set; }
        public string Hours { get; set; }
        public string Days { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public string ShortDescription { get; set; }
        public string ApplicationDeadline { get; set; }
        public List<CourseDetailDto> CourseDetails { get; set; }
        public string Instructor { get; set; }
        public string InstructorPosition { get; set; }
        public string InstructorImage { get; set; }
        public string InstructorBio { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string BehanceUrl { get; set; }
        public string DribbbleUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public Application Application { get; set; }
        public int OrderNo { get; set; }
    }
    public class CourseDetailDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderNo { get; set; }
    }
}
