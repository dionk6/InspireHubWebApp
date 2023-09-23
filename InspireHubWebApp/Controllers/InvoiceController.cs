using DinkToPdf;
using DinkToPdf.Contracts;
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
        private readonly IConverter _converter;

        public InvoiceController(DataContext context,
                                IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        public IActionResult Index()
        {
            var model = _context.Invoices
                        .Include(t => t.Student)
                        .Where(t => t.IsDeleted == false)
                        .OrderByDescending(t => t.Year)
                            .ThenByDescending(t => t.InvoiceNo)
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

                model.Description = student.Training.Title;
                model.Price = student.Price.ToString();
            }
            model.InvoiceDate = DateTime.Now.ToString("dd.MM.yyyy");
            model.Year = DateTime.Now.Year;
            var invoices = _context.Invoices
                        .Where(t => t.IsDeleted == false && t.Year == DateTime.Now.Year)
                        .ToList();
            if(invoices.Count > 0)
            {
                model.InvoiceNo = invoices.Max(t => t.InvoiceNo) + 1;
            }
            else
            {
                model.InvoiceNo = 1;
            }

            var students = _context.Students
                            .Include(t => t.Invoice)
                            .Where(t => t.IsDeleted == false && t.Invoice.Count() == 0)
                            .OrderByDescending(t => t.CreateDate)
                            .Select(t => new SelectDto
                            {
                                Label = t.FirstName+" "+t.LastName,
                                Value = t.Id.ToString()
                            })
                            .ToList();
            ViewBag.students = new SelectList(students, "Value", "Label");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Invoice model)
        {
            model.Id = 0;
            model.IsDeleted = false;
            model.CreatedDate = DateTime.Now;

            _context.Invoices.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var model = _context.Invoices.Find(id);
            var students = _context.Students
                            .Include(t => t.Invoice)
                            .Where(t => t.IsDeleted == false)
                            .OrderByDescending(t => t.CreateDate)
                            .Select(t => new SelectDto
                            {
                                Label = t.FirstName + " " + t.LastName,
                                Value = t.Id.ToString()
                            })
                            .ToList();
            ViewBag.students = new SelectList(students, "Value", "Label");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Invoice model)
        {
            model.ModifiedDate = DateTime.Now;

            _context.Invoices.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var model = _context.Invoices.Find(id);
            model.IsDeleted = true;

            _context.Invoices.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult PrintInvoice(int id)
        {
            var invoice = _context.Invoices
                        .Include(t => t.Student)
                        .FirstOrDefault(t => t.Id == id);

            var fileName = "Fatura Inspire Hub - "+invoice.Student.FirstName+" "+invoice.Student.LastName;
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 5, Bottom = 5 },
                DocumentTitle = fileName,
                //Out = @"D:\PDFCreator\Employee_Report.pdf" // USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };


            var path = Path.Combine(Directory.GetCurrentDirectory(), "ReportsViews", "InvoiceView.html");
            string Body = System.IO.File.ReadAllText(path);

            Body = Body.Replace("{{invoiceDate}}", invoice.InvoiceDate);
            Body = Body.Replace("{{invoiceNo}}", invoice.InvoiceNo+"/"+invoice.Year);
            Body = Body.Replace("{{studentName}}", invoice.Student.FirstName+" "+invoice.Student.LastName);
            Body = Body.Replace("{{studentAddress}}", invoice.StudentAddress);
            Body = Body.Replace("{{description}}", invoice.Description);
            Body = Body.Replace("{{month}}", invoice.Month);
            Body = Body.Replace("{{price}}", invoice.Price);


            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = Body,
                //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(),"ReportsViews", "assets", "styles.css") },
                //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Generated on "+DateTime.Now.ToString("dd.MM.yyyy") }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            /*var fileName = "Projects Report.pdf";
            var stream = new MemoryStream(file);
            string mimeType = "application/pdf";
            return new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = fileName
            };*/
            return File(file, "application/pdf");
        }
    }
}
