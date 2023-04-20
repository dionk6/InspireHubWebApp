using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InspireHubWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger,
                              IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            /*var contact = new Application();
            contact.CourseTitle = "Graphic Design Training";
            contact.FirstName = "Dion";
            contact.LastName = "Kuka";
            contact.Email = "kukadion1@gmail.com";
            contact.Phone = "+38344307373";
            var message = $"Hello," +
                             $"<br /> <br />" +
                             $"A new student has just applied to a course." +
                             $"<br /> <br />" +
                             $"Fullname: <b>{contact.FirstName + " " + contact.LastName}</b> " +
                             $"<br />" +
                             $"Email: {contact.Email}" +
                             $"<br />" +
                             $"Phone: {contact.Phone}" +
                             $"<br />" +
                             $"Course: <b>{contact.CourseTitle}</b> ";
            contact.Message = message;
            await _emailService.SendEmailAsync(contact);*/
            return View();
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