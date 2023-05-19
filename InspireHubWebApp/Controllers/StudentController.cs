using AutoMapper;
using InspireHubWebApp.DTOs;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public StudentController(DataContext context,
                                IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index(int id = 0)
        {
            var model = new StudentListDto();
            var students = _context.Students
                            .Include(t => t.Training)
                            .Where(t => t.IsDeleted == false && (id == 0 ? t.Id > 0 : t.TrainingId == id))
                            .Select(t => new StudentDto
                            {
                                Id = t.Id,
                                FirstName = t.FirstName,
                                LastName = t.LastName,
                                Email = t.Email,
                                Phone = t.Phone,
                                Price = t.Price,
                                Status = t.IsConfirmed,
                                IsPaid = t.IsPaid.HasValue ? t.IsPaid.Value : false,
                                Training = t.Training.Title,
                                DateApplied = t.CreateDate.ToString("dd MMMM yyyy"),
                                CreatedDate = t.CreateDate
                            })
                            .OrderByDescending(t => t.CreatedDate)
                            .ToList();
            model.Students = students;
            var trainings = _context.Training
                                .Where(t => t.IsDeleted == false)
                                .OrderBy(t => t.OrderNo)
                                .ToList();

            ViewBag.trainings = new SelectList(trainings, "Id", "Title");
            model.TrainingId = id;
            return View(model);
        }

        public IActionResult Add()
        {
            var model = new Application();

            var trainings = _context.Training
                                .Where(t => t.IsDeleted == false)
                                .OrderBy(t => t.OrderNo)
                                .ToList();

            ViewBag.trainings = new SelectList(trainings, "Id", "Title");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Application dto)
        {
            var training = _context.Training.Find(dto.TrainingId);
            var model = _mapper.Map<Student>(dto);

            model.Price = Decimal.Round(training.Price - training.DiscountPrice);
            model.IsConfirmed = true;
            model.CreateDate = DateTime.Now;
            model.IsDeleted = false;

            _context.Students.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<string> ApproveStudent(int id)
        {
            var student = _context.Students.Find(id);
            student.IsConfirmed = true;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return student.FirstName + " " + student.LastName + " is confirmed";
        }

        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);
            student.IsDeleted = true;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var model = _context.Students
                            .Include(t => t.Training)
                            .First(t => t.Id == id);
            var dto = _mapper.Map<Application>(model);
            dto.CourseTitle = model.Training.Title;

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Application dto)
        {
            var model = _mapper.Map<Student>(dto);
            model.ModifiedDate = DateTime.Now;

            _context.Students.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    } 
}
