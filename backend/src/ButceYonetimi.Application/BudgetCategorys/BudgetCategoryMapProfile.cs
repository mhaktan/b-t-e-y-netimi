using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.BudgetCategorys.Dto;

namespace ButceYonetimi.BudgetCategorys
{
    public class BudgetCategoryMapProfile : Profile
    {
        public BudgetCategoryMapProfile()
        {
            CreateMap<BudgetCategory, BudgetCategoryDto>();
            CreateMap<CreateBudgetCategoryDto, BudgetCategory>();
            CreateMap<BudgetCategoryDto, BudgetCategory>();
        }
    }
}
