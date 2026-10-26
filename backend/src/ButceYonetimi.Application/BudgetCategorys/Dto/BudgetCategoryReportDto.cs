using System;
using System.Collections.Generic;
using ButceYonetimi.Budgets.Dto;
using ButceYonetimi.BudgetRequestItems.Dto;
using ButceYonetimi.Expenses.Dto;

namespace ButceYonetimi.BudgetCategorys.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class BudgetCategoryReportDto
    {
        public BudgetCategoryDto Data { get; set; }
        public List<BudgetDto> Budgets { get; set; }
        public List<BudgetRequestItemDto> BudgetRequestItems { get; set; }
        public List<ExpenseDto> Expenses { get; set; }
    }
}
