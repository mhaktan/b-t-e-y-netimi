using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ButceYonetimi.Expenses.Dto
{
    [AutoMapFrom(typeof(Entities.Expense))]
    public class ExpenseDto : EntityDto<long>
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int Currency { get; set; }

        public decimal? AmountTRY { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string ReceiptNo { get; set; }

        public string Notes { get; set; }

        public int Status { get; set; }

        public string RevisionNote { get; set; }

        public long? ExpenseApproverId { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long BudgetId { get; set; }

        public string BudgetDisplay { get; set; }

        public long BudgetCategoryId { get; set; }

        public string BudgetCategoryDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}