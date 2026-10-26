using System;
using Abp.Application.Services.Dto;

namespace ButceYonetimi.BudgetRequestItems.Dto
{
    public class PagedBudgetRequestItemResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? BudgetRequestId { get; set; }
        public long? BudgetCategoryId { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public DateTime? PlannedDate { get; set; }
        public string Notes { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public decimal? QuantityFrom { get; set; }
        public decimal? QuantityTo { get; set; }
        public decimal? UnitPriceFrom { get; set; }
        public decimal? UnitPriceTo { get; set; }
        public DateTime? PlannedDateFrom { get; set; }
        public DateTime? PlannedDateTo { get; set; }
    }
}
