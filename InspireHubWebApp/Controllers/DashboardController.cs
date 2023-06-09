using InspireHubWebApp.DTOs;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DataContext _context;

        public DashboardController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardDto();

            model.Trainings = _context.Training.Where(t => t.IsDeleted == false).Count();
            model.Students = _context.Students.Where(t => t.IsDeleted == false).Count();
            model.UnpaidAmount = _context.Students.Where(t => t.IsDeleted == false && t.IsPaid != true).Sum(x => x.Price);
            model.Invoices = _context.Invoices.Where(t => t.IsDeleted == false).Count();

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            List<ChartKeyValue> chartData = new List<ChartKeyValue>();
            var students = _context.Students.Where(t => t.IsDeleted == false).ToList();
            foreach (var student in students)
            {
                var existingChart = chartData.Where(t => t.x == student.CreateDate.ToString("MM-dd-yyyy") + " GMT").FirstOrDefault();
                if (existingChart != null)
                {
                    existingChart.y += 1;
                }
                else
                {
                    chartData.Add(new ChartKeyValue
                    {
                        x = student.CreateDate.ToString("MM-dd-yyyy") + " GMT",
                        y = 1
                    });
                }
            }

            return Json(chartData);
        }

    }
}
