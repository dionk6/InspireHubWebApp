using InspireHubWebApp.DTOs;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        private readonly DataContext _context;

        public TrainingController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Training
                                .Where(t => t.IsDeleted == false)
                                .Select(t => new TrainingDto
                                {
                                    Id = t.Id,
                                    Title = t.Title,
                                    Instructor = t.Instructor,
                                    Price = t.Price,
                                    OrderNo = t.OrderNo,
                                    CreateDate = t.CreatedDate.ToString("dd/MM/yyyy")
                                })
                                .ToList();
            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }
    }
}
