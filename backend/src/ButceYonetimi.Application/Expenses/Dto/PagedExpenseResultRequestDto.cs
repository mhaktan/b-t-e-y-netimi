using System;
using Abp.Application.Services.Dto;

namespace ButceYonetimi.Expenses.Dto
{
    public class PagedExpenseResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? BudgetId { get; set; }
        public long? BudgetCategoryId { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public int? Currency { get; set; }
        public decimal? AmountTRY { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string ReceiptNo { get; set; }
        public string Notes { get; set; }
        public int? Status { get; set; }
        public string RevisionNote { get; set; }
        public long? ExpenseApproverId { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public decimal? AmountTRYFrom { get; set; }
        public decimal? AmountTRYTo { get; set; }
        public DateTime? ExpenseDateFrom { get; set; }
        public DateTime? ExpenseDateTo { get; set; }
        public long? ExpenseApproverIdFrom { get; set; }
        public long? ExpenseApproverIdTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
