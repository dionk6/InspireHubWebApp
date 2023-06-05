using InspireHubWebApp.DTOs;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly DataContext _context;

        public InvoiceController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _context.Invoices
                        .Include(t => t.Student)
                        .Where(t => t.IsDeleted == false)
                        .ToList();

            return View(model);
        }

        public IActionResult Add(int id = 0)
        {
            var model = new Invoice();
            if(id > 0)
            {
                model.StudentId = id;
                var student = _context.Students
                            .Include(t => t.Training)
                            .First(t => t.Id == id);

                model.Description = "Trajnimi - "+student.Training.Title;
                model.Price = student.Price.ToString();
            }
            model.InvoiceDate = DateTime.Now.ToString("dd.MM.yyyy");


            var students = _context.Students
                            .Where(t => t.IsDeleted == false)
                            .Select(t => new SelectDto
                            {
                                Label = t.FirstName+" "+t.LastName,
                                Value = t.Id.ToString()
                            })
                            .ToList();
            ViewBag.students = new SelectList(students, "Value", "Label");

            return View(model);
        }
    }
}
