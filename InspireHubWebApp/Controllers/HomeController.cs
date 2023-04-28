using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;
using InspireHubWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InspireHubWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;
        private readonly reCaptchaService _reCaptchaService;

        public HomeController(ILogger<HomeController> logger,
                              IEmailService emailService,
                              reCaptchaService reCaptchaService)
        {
            _logger = logger;
            _emailService = emailService;
            _reCaptchaService = reCaptchaService;
        }

        public async Task<IActionResult> Index()
        {
            var contact = new Application();
            return View(contact);
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