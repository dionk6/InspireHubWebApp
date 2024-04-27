using DinkToPdf;
using DinkToPdf.Contracts;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private readonly DataContext _context;
        private readonly IConverter _converter;

        public ContractController(DataContext context,
                                  IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        public IActionResult Index()
        {
            var model = _context.Contracts.Where(t => t.IsDeleted == false).ToList();
            return View(model);
        }

        public IActionResult Add(int id = 0)
        {
            if(id == 0)
            {
                return View();
            }
            var student = _context.Students
                        .Include(t => t.Training)
                        .FirstOrDefault(t => t.Id == id);
            var model = new Contract();
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.Price = student.Price.ToString();
            model.StartDate = student.Training.StartDate.ToString("dd/MM/yyyy");
            model.EndDate = student.Training.EndDate.ToString("dd/MM/yyyy");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Contract model)
        {
            model.Id = 0;
            model.IsDeleted = false;
            model.CreatedDate = DateTime.Now;

            _context.Contracts.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var model = _context.Contracts.Find(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Contract model)
        {
            model.ModifiedDate = DateTime.Now;

            _context.Contracts.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteContract(int id)
        {
            var model = _context.Contracts.Find(id);
            model.IsDeleted = true;

            _context.Contracts.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PrintContract(int id)
        {
            var model = _context.Contracts.Find(id);
            var fileName = "Kontrate e Sherbimit Inspire Hub - " + model.FirstName + " " + model.LastName;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "ReportsViews", "ContractView.html");
            string Body = System.IO.File.ReadAllText(path);

            Body = Body.Replace("{{contractTable}}", model.ContractDate != null ? model.ContractDate : "__________");
            Body = Body.Replace("{{fullName}}", model.FirstName +" "+model.LastName);
            Body = Body.Replace("{{address}}", model.Address != null ? model.Address : "______________________");
            Body = Body.Replace("{{personalNumber}}", model.PersonalNumber != null ? model.PersonalNumber : "_____________");
            Body = Body.Replace("{{price}}", model.Price != null ? model.Price : "____");
            Body = Body.Replace("{{startDate}}", model.StartDate != null ? model.StartDate : "_________");
            Body = Body.Replace("{{endDate}}", model.EndDate != null ? model.EndDate : "_________");

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox" }
            });
            var page = await browser.NewPageAsync();
            await page.SetContentAsync(Body);

            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                DisplayHeaderFooter = false,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "30px",
                    Right = "0px",
                    Bottom = "35px",
                    Left = "0px"
                },
            });

            return File(pdfBytes, "application/pdf");
        }

    }
}
