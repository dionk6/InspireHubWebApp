﻿using InspireHubWebApp.DTOs;
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
            var contact = new Application();
            return View(contact);
        }

        [HttpPost]
        public async Task<IActionResult> SendApplication(Application model)
        {
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