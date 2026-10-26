using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.BudgetRequests.Dto;

namespace ButceYonetimi.BudgetRequests
{
    public class BudgetRequestMapProfile : Profile
    {
        public BudgetRequestMapProfile()
        {
            CreateMap<BudgetRequest, BudgetRequestDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.BudgetDisplay, o => o.MapFrom(s => s.Budget != null ? s.Budget.Title.ToString() : null))
                .ForMember(d => d.DepartmentDisplay, o => o.MapFrom(s => s.Department != null ? s.Department.Name.ToString() : null));
            CreateMap<CreateBudgetRequestDto, BudgetRequest>();
            CreateMap<BudgetRequestDto, BudgetRequest>();
        }
    }
}
