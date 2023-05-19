using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;
using InspireHubWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InspireHubWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;
        private readonly reCaptchaService _reCaptchaService;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger,
                              IEmailService emailService,
                              reCaptchaService reCaptchaService,
                              DataContext context)
        {
            _logger = logger;
            _emailService = emailService;
            _reCaptchaService = reCaptchaService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var contact = new Application();

            var model = _context.Training
                            .AsNoTracking()
                            .Include(t => t.TrainingCourseDetails)
                                .ThenInclude(t => t.CourseDetail)
                            .Where(t => t.IsDeleted == false && t.Show == true)
                            .Select(t => new TrainingView
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Dates = t.StartDate.ToString("dd MMMM yyyy")+" - "+t.EndDate.ToString("dd MMMM yyyy"),
                                Hours = t.Hours,
                                Days = t.Days,
                                Price = Decimal.Round(t.Price),
                                FinalPrice = t.DiscountPrice > 0 ? Decimal.Round(t.Price - t.DiscountPrice) : 0,
                                ShortDescription = t.ShortDescription,
                                ApplicationDeadline = t.ApplicationDeadline,
                                CourseDetails = t.TrainingCourseDetails
                                                    .Select(x => new CourseDetailDto
                                                    {
                                                        Title = x.CourseDetail.Title,
                                                        Description = x.Description,
                                                        OrderNo = x.OrderNo,
                                                    })
                                                    .OrderBy(x => x.OrderNo)
                                                    .ToList(),
                                Instructor = t.Instructor,
                                InstructorPosition = t.InstructorPosition,
                                InstructorImage = t.InstructorImage,
                                InstructorBio = t.InstructorBio,
                                FacebookUrl = t.FacebookUrl,
                                InstagramUrl = t.InstagramUrl,
                                BehanceUrl = t.BehanceUrl,
                                DribbbleUrl = t.DribbbleUrl,
                                LinkedinUrl = t.LinkedinUrl,
                                Application = new Application
                                {
                                    TrainingId = t.Id,
                                    CourseTitle = t.Title,
                                    Price = Decimal.Round(t.Price - t.DiscountPrice)
                                },
                                OrderNo = t.OrderNo
                            })
                            .OrderBy(x => x.OrderNo)
                            .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendApplication(Application model)
        {
            //google reCaptcha confirmation
            var reCaptcharesult = await _reCaptchaService.tokenVerify(model.token);
            if (!reCaptcharesult.success && reCaptcharesult.score <= 0.5)
            {
                //ModelState.AddModelError(string.Empty, "You are not a human.");
                return RedirectToAction("Index");
            }

            var newStudent = new Student();
            newStudent.TrainingId = model.TrainingId.Value;
            newStudent.FirstName = model.FirstName;
            newStudent.LastName = model.LastName;
            newStudent.Email = model.Email;
            newStudent.Phone = model.Phone;
            newStudent.Price = model.Price.Value;
            newStudent.IsConfirmed = false;
            newStudent.IsPaid = false;
            newStudent.IsDeleted = false;
            newStudent.CreateDate = DateTime.Now;

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();

            var message = $"Hello," +
                             $"<br /> <br />" +
                             $"A new student has just applied to a course." +
                             $"<br /> <br />" +
                             $"Fullname: <b>{model.FirstName + " " + model.LastName}</b> " +
                             $"<br />" +
                             $"Email: {model.Email}" +
                             $"<br />" +
                             $"Phone: {model.Phone}" +
                             $"<br />" +
                             $"Course: <b>{model.CourseTitle}</b> ";
            model.Message = message;
            await _emailService.SendEmailAsync(model);
            TempData["id"] = 1;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendContact(Application model)
        {
            //google reCaptcha confirmation
            var reCaptcharesult = await _reCaptchaService.tokenVerify(model.token);
            if (!reCaptcharesult.success && reCaptcharesult.score <= 0.5)
            {
                //ModelState.AddModelError(string.Empty, "You are not a human.");
                return RedirectToAction("Index");
            }

            model.CourseTitle = "Message";
            var message = $"Hello," +
                             $"<br /> <br />" +
                             $"Fullname: <b>{model.FirstName}</b> " +
                             $"<br />" +
                             $"Email: {model.Email}" +
                             $"<br />" +
                             $"Message:" +
                             $"<br />" +
                             $"{model.Message}";
            model.Message = message;
            await _emailService.SendEmailAsync(model);
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}