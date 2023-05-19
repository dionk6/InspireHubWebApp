using DinkToPdf;
using DinkToPdf.Contracts;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Contract model)
        {
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

        public IActionResult PrintContract(int id)
        {
            var model = _context.Contracts.Find(id);
            var fileName = "Kontrate e Sherbimit Inspire Hub - " + model.FirstName + " " + model.LastName;
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = fileName,
                //Out = @"D:\PDFCreator\Employee_Report.pdf" // USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };


            var path = Path.Combine(Directory.GetCurrentDirectory(), "ReportsViews", "ContractView.html");
            string Body = System.IO.File.ReadAllText(path);

            Body = Body.Replace("{{contractTable}}", model.ContractDate != null ? model.ContractDate : "__________");
            Body = Body.Replace("{{fullName}}", model.FirstName +" "+model.LastName);
            Body = Body.Replace("{{address}}", model.Address != null ? model.Address : "______________________");
            Body = Body.Replace("{{personalNumber}}", model.PersonalNumber != null ? model.PersonalNumber : "_____________");
            Body = Body.Replace("{{price}}", model.Price != null ? model.Price : "____");
            Body = Body.Replace("{{startDate}}", model.StartDate != null ? model.StartDate : "_________");
            Body = Body.Replace("{{endDate}}", model.EndDate != null ? model.EndDate : "_________");

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                /*HeaderSettings = new HeaderSettings
                {
                    Spacing
                },*/
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
