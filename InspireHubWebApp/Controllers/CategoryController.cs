using AutoMapper;
using InspireHubWebApp.DTOs;
using InspireHubWebApp.Interfaces;
using InspireHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspireHubWebApp.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUploadService _uploadService;

        public CategoryController(DataContext context,
                                  IMapper mapper,
                                  IUploadService uploadService)
        {
            _context = context;
            _mapper = mapper;
            _uploadService = uploadService;
        }

        public IActionResult Index()
        {
            var model = _context.CourseDetail
                                .Where(t => t.IsDeleted == false)
                                .Select(t => new CategoryDto
                                {
                                    Id = t.Id,
                                    CreatedDate = t.CreatedDate,
                                    Title = t.Title,
                                    OrderNo = t.OrderNo
                                })
                                .ToList();
            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryDto dto)
        {
            var model = _mapper.Map<CourseDetail>(dto);
            model.CreatedDate = DateTime.Now;

            _context.CourseDetail.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var model = _context.CourseDetail.Find(id);
            var dto = _mapper.Map<CategoryDto>(model);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CategoryDto dto)
        {
            var model = _mapper.Map<CourseDetail>(dto);
            model.ModifiedDate = DateTime.Now;

            _context.CourseDetail.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var model = _context.CourseDetail.Find(id);

            model.IsDeleted = true;
            _context.CourseDetail.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
