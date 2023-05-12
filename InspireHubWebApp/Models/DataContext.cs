using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InspireHubWebApp.Models
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext()
        {

        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Training> Training { get; set; }
        public DbSet<CourseDetail> CourseDetail { get; set; }
        public DbSet<TrainingCourseDetails> TrainingCourseDetails { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
