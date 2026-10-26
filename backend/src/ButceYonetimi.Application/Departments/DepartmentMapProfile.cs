using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.Departments.Dto;

namespace ButceYonetimi.Departments
{
    public class DepartmentMapProfile : Profile
    {
        public DepartmentMapProfile()
        {
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<DepartmentDto, Department>();
        }
    }
}
