using System;
using Abp.Application.Services.Dto;

namespace ButceYonetimi.Budgets.Dto
{
    public class PagedBudgetResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? BudgetPeriodId { get; set; }
        public long? DepartmentId { get; set; }
        public long? BudgetCategoryId { get; set; }
        public string Title { get; set; }
        public decimal? AllocatedAmount { get; set; }
        public int? Currency { get; set; }
        public decimal? AllocatedAmountTRY { get; set; }
        public string Description { get; set; }
        public bool? IsAutoCreated { get; set; }
        public int? Status { get; set; }
        public string RevisionNote { get; set; }
        public long? FinanceDirectorId { get; set; }
        public decimal? AllocatedAmountFrom { get; set; }
        public decimal? AllocatedAmountTo { get; set; }
        public decimal? AllocatedAmountTRYFrom { get; set; }
        public decimal? AllocatedAmountTRYTo { get; set; }
        public long? FinanceDirectorIdFrom { get; set; }
        public long? FinanceDirectorIdTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
