using System;
using Abp.Application.Services.Dto;

namespace ButceYonetimi.BudgetRequests.Dto
{
    public class PagedBudgetRequestResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? BudgetId { get; set; }
        public long? DepartmentId { get; set; }
        public string Title { get; set; }
        public string RequestNo { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? Currency { get; set; }
        public decimal? TotalAmountTRY { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? Status { get; set; }
        public string RevisionNote { get; set; }
        public decimal? FinanceApprovalThreshold { get; set; }
        public long? ManagerApproverId { get; set; }
        public string FinanceApproverRole { get; set; }
        public decimal? TotalAmountFrom { get; set; }
        public decimal? TotalAmountTo { get; set; }
        public decimal? TotalAmountTRYFrom { get; set; }
        public decimal? TotalAmountTRYTo { get; set; }
        public DateTime? RequestDateFrom { get; set; }
        public DateTime? RequestDateTo { get; set; }
        public decimal? FinanceApprovalThresholdFrom { get; set; }
        public decimal? FinanceApprovalThresholdTo { get; set; }
        public long? ManagerApproverIdFrom { get; set; }
        public long? ManagerApproverIdTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
