using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.BudgetPeriods.Dto;

namespace ButceYonetimi.BudgetPeriods
{
    public class BudgetPeriodMapProfile : Profile
    {
        public BudgetPeriodMapProfile()
        {
            CreateMap<BudgetPeriod, BudgetPeriodDto>();
            CreateMap<CreateBudgetPeriodDto, BudgetPeriod>();
            CreateMap<BudgetPeriodDto, BudgetPeriod>();
        }
    }
}
