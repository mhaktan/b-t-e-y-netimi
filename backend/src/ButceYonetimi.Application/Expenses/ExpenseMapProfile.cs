using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.Expenses.Dto;

namespace ButceYonetimi.Expenses
{
    public class ExpenseMapProfile : Profile
    {
        public ExpenseMapProfile()
        {
            CreateMap<Expense, ExpenseDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.BudgetDisplay, o => o.MapFrom(s => s.Budget != null ? s.Budget.Title.ToString() : null))
                .ForMember(d => d.BudgetCategoryDisplay, o => o.MapFrom(s => s.BudgetCategory != null ? s.BudgetCategory.Name.ToString() : null));
            CreateMap<CreateExpenseDto, Expense>();
            CreateMap<ExpenseDto, Expense>();
        }
    }
}
