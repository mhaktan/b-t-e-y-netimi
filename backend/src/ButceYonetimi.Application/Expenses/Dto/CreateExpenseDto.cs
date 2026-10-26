using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ButceYonetimi.Expenses.Dto
{
    [AutoMapTo(typeof(Entities.Expense))]
    public class CreateExpenseDto
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int Currency { get; set; }

        public decimal? AmountTRY { get; set; }

        public DateTime ExpenseDate { get; set; }

        [MaxLength(100)]
        public string ReceiptNo { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public int Status { get; set; }

        [MaxLength(500)]
        public string RevisionNote { get; set; }

        public long? ExpenseApproverId { get; set; }

        public long BudgetId { get; set; }

        public long BudgetCategoryId { get; set; }

    }
}