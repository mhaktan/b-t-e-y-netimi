using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.BudgetRequestItems.Dto;

namespace ButceYonetimi.BudgetRequestItems
{
    public class BudgetRequestItemMapProfile : Profile
    {
        public BudgetRequestItemMapProfile()
        {
            CreateMap<BudgetRequestItem, BudgetRequestItemDto>()
                .ForMember(d => d.BudgetRequestDisplay, o => o.MapFrom(s => s.BudgetRequest != null ? s.BudgetRequest.Title.ToString() : null))
                .ForMember(d => d.BudgetCategoryDisplay, o => o.MapFrom(s => s.BudgetCategory != null ? s.BudgetCategory.Name.ToString() : null));
            CreateMap<CreateBudgetRequestItemDto, BudgetRequestItem>();
            CreateMap<BudgetRequestItemDto, BudgetRequestItem>();
        }
    }
}
