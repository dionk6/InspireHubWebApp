using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;

namespace InspireHubWebApp.Services
{
    public class MainService : IMainService
    {
        private readonly DataContext _context;

        public MainService(DataContext context)
        {
            _context = context;
        }

        public int CountNewStudents()
        {
            var students = _context.Students
                        .Where(t => t.IsDeleted == false && (t.IsViewed == null || t.IsViewed == false))
                        .Count();

            return students;
        }
    }
}
