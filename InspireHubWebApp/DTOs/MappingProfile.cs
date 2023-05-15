using AutoMapper;
using InspireHubWebApp.Models;

namespace InspireHubWebApp.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Training, TrainingDto>();
            CreateMap<TrainingDto, Training>();

            CreateMap<CourseDetail, CategoryDto>();
            CreateMap<CategoryDto, CourseDetail>();

            CreateMap<Student, Application>();
            CreateMap<Application, Student>();
        }
    }
}
