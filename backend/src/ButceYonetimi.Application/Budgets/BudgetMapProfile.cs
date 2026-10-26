using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.Budgets.Dto;

namespace ButceYonetimi.Budgets
{
    public class BudgetMapProfile : Profile
    {
        public BudgetMapProfile()
        {
            CreateMap<Budget, BudgetDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.BudgetPeriodDisplay, o => o.MapFrom(s => s.BudgetPeriod != null ? s.BudgetPeriod.Name.ToString() : null))
                .ForMember(d => d.DepartmentDisplay, o => o.MapFrom(s => s.Department != null ? s.Department.Name.ToString() : null))
                .ForMember(d => d.BudgetCategoryDisplay, o => o.MapFrom(s => s.BudgetCategory != null ? s.BudgetCategory.Name.ToString() : null));
            CreateMap<CreateBudgetDto, Budget>();
            CreateMap<BudgetDto, Budget>();
        }
    }
}
