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
            newStudent.IsViewed = false;
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

            var template = await _context.Templates.FindAsync(1);
            await _emailService.SendMessageAsync(template.Subject, model.Email, template.Body, null);

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

        //public async Task<IActionResult> SeedTemplates()
        //{
        //    var template = await _context.Templates.FindAsync(1);
        //    template.Subject = "Aplikimi përfundoi me sukses";
        //    template.Body = "<!DOCTYPE html> <html lang=\"en\"> <head> <meta charset=\"UTF-8\" /> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" /> <title>Newsletter</title> <style type=\"text/css\"> body { text-align: center; margin: 0; padding: 0; -webkit-text-size-adjust: 100%; width: 100%; max-width: 100%; background-color: #f0f0f0; } h1 { font-size: 44px; line-height: 24px; font-weight: 900; text-decoration: none; color: #021255; margin: 30px 0px 35px 0px; } hr { width: 18%; height: 3px; background-color: #021255; border: none; } .footer { margin-top: 30px; text-align: center; } .social-icons { margin-top: 10px; text-align: center; } .social-icons td { display: inline-block; margin: 0 20px; } .social-icons img { max-width: 50px; height: auto; } </style> </head> <body align=\"center\"> <!-- Fallback force center content --> <div style=\" text-align: center; width: 800px; max-width: 800px; margin: 0 auto; background-image: url('https://inspirehub.info/img/bgInspire.png'); background-position: top; background-repeat: no-repeat; background-size: contain; \" > <!-- Start container for logo --> <table align=\"center\" style=\"text-align: center; vertical-align: top\"> <tbody> <tr> <td style=\" width: 100%; vertical-align: top; padding-left: 0; padding-right: 0; padding-top: 15px; padding-bottom: 15px; \" width=\"100%\" > <!-- Your logo is here --> <img style=\" width: 80px; max-width: 80px; height: auto; text-align: center; color: #ffffff; \" alt=\"Logo\" src=\"https://inspirehub.info/img/emailLogo.png\" align=\"center\" /> </td> </tr> </tbody> </table> <!-- End container for logo --> <!-- Start single column section --> <table align=\"center\" style=\" text-align: center; vertical-align: top; width: 100%; max-width: 100%; background-color: transparent; \" > <tbody> <tr> <td style=\" width: 100%; vertical-align: top; padding-left: 30px; padding-right: 30px; padding-bottom: 40px; \" > <h1>Aplikimi përfundoi me sukses</h1> <hr /> <p style=\" font-size: 22px; line-height: 28px; font-weight: 600; text-decoration: none; color: #021255; \" > Faleminderit që zgjodhët InspireHub për aftësimin tuaj profesional.<br /> Së shpejti ju kontaktojmë për konfirmim përmes telefonit! </p> </td> </tr> </tbody> </table> <!-- End single column section --> <!-- Hero image --> <img style=\" width: 600px; max-width: 600px; height: 350px; max-height: 350px; text-align: center; margin-bottom: 100px; \" alt=\"Hero image\" src=\"https://inspirehub.info/img/Ilustrim.png\" align=\"center\" width=\"600\" height=\"350\" /> <!-- Hero image --> <!-- Start footer --> <table align=\"center\"> <tbody class=\"footer\"> <tr class=\"social-icons\"> <td> <a href=\"https://www.facebook.com/inspirehubacademy\" ><img src=\"https://inspirehub.info/img/blackFacebook.png\" alt=\"Facebook\" /></a> </td> <td> <a href=\"https://www.instagram.com/inspirehub_/\" ><img src=\"https://inspirehub.info/img/blackInstagram.png\" alt=\"Instagram\" /></a> </td> <td> <a href=\"https://www.linkedin.com/company/inspire-hub-learningcenter/\" ><img src=\"https://inspirehub.info/img/blackLinkedin.png\" alt=\"Linkedin\" /></a> </td> </tr> </tbody> </table> <!-- End footer --> </div> </body> </html>";
        //    _context.Templates.Update(template);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> TestTemplate()
        {
            var template1 = await _context.Templates.FindAsync(1);
            await _emailService.SendMessageAsync(template1.Subject, "kukadion1@gmail.com,bardhmurti@gmail.com", template1.Body, null);

            return RedirectToAction("Index");
        }
    }
}