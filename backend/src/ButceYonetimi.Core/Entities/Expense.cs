using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ButceYonetimi.Entities
{
    // State Machine: status — Draft → PendingApproval → Approved → Rejected
    // Initial: Draft | Transitions: Draft→PendingApproval[Submit], PendingApproval→Approved[Approve], PendingApproval→Rejected[Reject], PendingApproval→Draft[Revise]
    [Table("Expenses")]
    public class Expense : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public ExpenseCurrency Currency { get; set; }

        public decimal? AmountTRY { get; set; }

        public DateTime ExpenseDate { get; set; }

        [MaxLength(100)]
        public string ReceiptNo { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public ExpenseStatus Status { get; set; }

        [MaxLength(500)]
        public string RevisionNote { get; set; }

        public long? ExpenseApproverId { get; set; }

        public long BudgetId { get; set; }

        [ForeignKey(nameof(BudgetId))]
        public virtual Budget Budget { get; set; }

        public long BudgetCategoryId { get; set; }

        [ForeignKey(nameof(BudgetCategoryId))]
        public virtual BudgetCategory BudgetCategory { get; set; }

    }
}