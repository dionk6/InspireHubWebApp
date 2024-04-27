using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp.Media;
using PuppeteerSharp;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConverter _converter;
        private readonly IEmailService _emailService;

        public StudentController(DataContext context,
                                IMapper mapper,
                                IConverter converter,
                                IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _converter = converter;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string subject, string emails, string message, IFormFile file)
        {
            await _emailService.SendMessageAsync(subject,emails,message,file);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(int id = 0)
        {
            var unviewedStudents = _context.Students
                                    .Where(t => t.IsDeleted == false && (t.IsViewed == false || t.IsViewed == null))
                                    .ToList();
            foreach (var item in unviewedStudents)
                item.IsViewed = true;

            if (unviewedStudents.Count() > 0)
            {
                _context.Students.UpdateRange(unviewedStudents);
                await _context.SaveChangesAsync();
            }

            var model = new StudentListDto();
            var students = _context.Students
                            .Include(t => t.Training)
                            .Include(t => t.Invoice)
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
                                CreatedDate = t.CreateDate,
                                HasInvoice = t.Invoice.Where(x => x.IsDeleted == false).Count() > 0
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

        public string checkConfirmed(bool isConfirmed)
        {
            return isConfirmed ? "Confirmed" : "Pending";
        }

        public string checkPaid(bool? isPaid)
        {
            return isPaid == true ? "Paid" : "Unpaid";
        }

        /*public IActionResult Print(int id)
        {
            string fileName = "Students";
            if (id != 0)
            {
                var model = _context.Training.Find(id);
                fileName = model.Title + " - Students";
            }

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = fileName,
                //Out = @"D:\PDFCreator\Employee_Report.pdf" // USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };

            var students = _context.Students
                            .Include(t => t.Training)
                            .Where(t => t.IsDeleted == false && (id == 0 ? t.Id > 0 : t.TrainingId == id))
                            .ToList();
            var mainTableData = "";
            foreach (var item in students)
            {
                mainTableData +=
                    $@"
                        <tr>
				            <td>{item.FirstName+" "+item.LastName}</td>
				            <td>{item.Email}</td>
				            <td>{item.Phone}</td>
				            <td>{item.Training.Title}</td>
				            <td>{item.Price} €</td>
				            <td>{item.CreateDate.ToString("dd MMMM yyyy")}</td>
                            <td>{checkConfirmed(item.IsConfirmed)}, {checkPaid(item.IsPaid)}</td>
			            </tr>
                    ";
            }

            var mainTable =
                $@"
                    <table>
	                    <thead>
		                    <tr>
			                    <th>Fullname</th>
			                    <th>Email</th>
			                    <th>Phone</th>
			                    <th>Training</th>
			                    <th>Price</th>
			                    <th>Date Applied</th>
			                    <th>Status</th>
		                    </tr>
	                    </thead>
	                    <tbody>
		                    {mainTableData}
	                    </tbody>
                    </table>       
                ";



            var path = Path.Combine(Directory.GetCurrentDirectory(), "ReportsViews", "StudentsView.html");
            string Body = System.IO.File.ReadAllText(path);

            Body = Body.Replace("{{studentsTitle}}", fileName);
            Body = Body.Replace("{{mainTable}}", mainTable);

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = Body,
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Generated on "+DateTime.Now.ToString("dd.MM.yyyy") }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            return File(file, "application/pdf");
        }*/

        public async Task<IActionResult> Print(int id)
        {
            string fileName = "Students";
            if (id != 0)
            {
                var model = _context.Training.Find(id);
                fileName = model.Title + " - Students";
            }

            var students = _context.Students
                            .Include(t => t.Training)
                            .Where(t => t.IsDeleted == false && (id == 0 ? t.Id > 0 : t.TrainingId == id))
                            .ToList();
            var mainTableData = "";
            foreach (var item in students)
            {
                mainTableData +=
                    $@"
                        <tr>
				            <td>{item.FirstName + " " + item.LastName}</td>
				            <td>{item.Email}</td>
				            <td>{item.Phone}</td>
				            <td>{item.Training.Title}</td>
				            <td>{item.Price} €</td>
				            <td>{item.CreateDate.ToString("dd MMMM yyyy")}</td>
                            <td>{checkConfirmed(item.IsConfirmed)}, {checkPaid(item.IsPaid)}</td>
			            </tr>
                    ";
            }

            var mainTable =
                $@"
                    <table>
	                    <thead>
		                    <tr>
			                    <th>Fullname</th>
			                    <th>Email</th>
			                    <th>Phone</th>
			                    <th>Training</th>
			                    <th>Price</th>
			                    <th>Date Applied</th>
			                    <th>Status</th>
		                    </tr>
	                    </thead>
	                    <tbody>
		                    {mainTableData}
	                    </tbody>
                    </table>       
                ";


            var path = Path.Combine(Directory.GetCurrentDirectory(), "ReportsViews", "StudentsView.html");
            string Body = System.IO.File.ReadAllText(path);

            Body = Body.Replace("{{studentsTitle}}", fileName);
            Body = Body.Replace("{{mainTable}}", mainTable);

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
                Landscape = true,
                DisplayHeaderFooter = false,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "10px",
                    Right = "30px",
                    Bottom = "0px",
                    Left = "30px"
                },
            });

            return File(pdfBytes, "application/pdf");
        }

    } 
}
