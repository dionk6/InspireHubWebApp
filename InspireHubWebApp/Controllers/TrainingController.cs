using AutoMapper;
using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Details(int id)
        {
            ViewBag.trainingId = id;
            var model = _context.TrainingCourseDetails
                                .Where(t => t.TrainingId == id)
                                .Include(t => t.Training)
                                .Include(t => t.CourseDetail)
                                .Select(t => new TrainingDetailDto
                                {
                                    Id = t.Id,
                                    TrainingId = t.TrainingId,
                                    Training = t.Training.Title,
                                    CourseDetail = t.CourseDetail.Title,
                                    Date = t.CreatedDate.ToString("dd/MM/yyyy"),
                                    OrderNo = t.OrderNo
                                })
                                .OrderBy(t => t.OrderNo)
                                .ToList();
                            
            return View(model);
        }

        public IActionResult AddDetail(int id)
        {
            var courseDetails = _context.CourseDetail
                                  .Include(t => t.TrainingCourseDetails)
                                  .Where(t => t.IsDeleted == false && t.TrainingCourseDetails.Select(x => x.TrainingId == id).Count() == 0)
                                  .OrderBy(t => t.OrderNo)
                                  .ToList();
            ViewBag.courseDetails = new SelectList(courseDetails, "Id", "Title");

            var dto = new TrainingDetailDto();
            var training = _context.Training.Find(id);
            dto.Training = training.Title;
            dto.TrainingId = training.Id;

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> AddDetail(TrainingDetailDto dto)
        {
            var model = new TrainingCourseDetails();
            model.TrainingId = dto.TrainingId.Value;
            model.CourseDetailId = dto.CourseDetailId;
            model.Description = dto.Description;
            model.OrderNo = dto.OrderNo;
            model.CreatedDate = DateTime.Now;

            _context.TrainingCourseDetails.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new {id = model.TrainingId });
        }

        public IActionResult UpdateDetail(int id)
        {
            var model = _context.TrainingCourseDetails
                                .Include(t => t.CourseDetail)
                                .Include(t => t.Training)
                                .Where(t => t.Id == id)
                                .FirstOrDefault();

            var dto = new TrainingDetailDto();
            dto.Id = model.Id;
            dto.Training = model.Training.Title;
            dto.TrainingId = model.Training.Id;
            dto.CourseDetail = model.CourseDetail.Title;
            dto.CourseDetailId = model.CourseDetail.Id;
            dto.OrderNo = model.OrderNo;
            dto.Description = model.Description;
            dto.CreateDate = model.CreatedDate;

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDetail(TrainingDetailDto dto)
        {
            var model = _context.TrainingCourseDetails.Find(dto.Id);
            model.TrainingId = dto.TrainingId.Value;
            model.CourseDetailId = dto.CourseDetailId;
            model.Description = dto.Description;
            model.OrderNo = dto.OrderNo;
            model.ModifiedDate = DateTime.Now;

            _context.TrainingCourseDetails.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.TrainingId });
        }

        public async Task<IActionResult> DeleteTrainingDetail(int id)
        {
            var model = _context.TrainingCourseDetails.Find(id);

            _context.TrainingCourseDetails.Remove(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.TrainingId });
        }

        

    }
}
