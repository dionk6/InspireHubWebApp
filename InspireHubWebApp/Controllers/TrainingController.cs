using AutoMapper;
using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;

        public TrainingController(DataContext context,
                                  IMapper mapper,
                                  IUploadService uploadService)
        {
            _context = context;
            _mapper = mapper;
            _uploadService = uploadService;
        }
        public IActionResult Index()
        {
            var model = _context.Training
                                .Where(t => t.IsDeleted == false)
                                .Select(t => new TrainingDto
                                {
                                    Id = t.Id,
                                    Title = t.Title,
                                    Instructor = t.Instructor,
                                    Price = Decimal.Round(t.Price - t.DiscountPrice),
                                    OrderNo = t.OrderNo,
                                    TrainingDates = t.StartDate.ToString("dd MMMM yyyy")+" - "+t.EndDate.ToString("dd MMMM yyyy")
                                })
                                .ToList();
            return View(model);
        }

        public IActionResult Add()
        {
            var dto = new TrainingDto();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TrainingDto dtoModel)
        {
            var model = _mapper.Map<Training>(dtoModel);

            var instructorPhoto = await _uploadService.Upload(dtoModel.InstructorPhoto,"Instructor",model.Id.ToString());
            model.InstructorImage = instructorPhoto;
            model.StartDate = DateTime.ParseExact(dtoModel.StartDateString,"dd/MM/yyyy",null);
            model.EndDate = DateTime.ParseExact(dtoModel.EndDateString, "dd/MM/yyyy", null);
            model.CreatedDate = DateTime.Now;
            model.IsDeleted = false;

            _context.Training.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            var model = _context.Training.Find(id);

            var dto = _mapper.Map<TrainingDto>(model);
            dto.StartDateString = model.StartDate.ToString("dd/MM/yyyy");
            dto.EndDateString = model.EndDate.ToString("dd/MM/yyyy");

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(TrainingDto dtoModel)
        {
            var model = _mapper.Map<Training>(dtoModel);
            if(dtoModel.InstructorUpdatePhoto != null)
            {
                var instructorPhoto = await _uploadService.Upload(dtoModel.InstructorUpdatePhoto, "Instructor", model.Id.ToString());
                model.InstructorImage = instructorPhoto;
            }
            model.StartDate = DateTime.ParseExact(dtoModel.StartDateString, "dd/MM/yyyy", null);
            model.EndDate = DateTime.ParseExact(dtoModel.EndDateString, "dd/MM/yyyy", null);
            model.ModifiedDate= DateTime.Now;

            _context.Training.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTraining(int id)
        {
            var model = _context.Training.Find(id);

            model.IsDeleted = true;
            _context.Training.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
