using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ButceYonetimi.Entities
{
    // State Machine: status — Draft → PendingApproval → Active → Closed → Cancelled
    // Initial: Draft | Transitions: Draft→PendingApproval[Submit], PendingApproval→Active[Approve], PendingApproval→Draft[Revise], Active→Closed[Close], *→Cancelled[Cancel]
    [Table("Budgets")]
    public class Budget : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        public decimal AllocatedAmount { get; set; }

        public BudgetCurrency Currency { get; set; }

        public decimal? AllocatedAmountTRY { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public bool? IsAutoCreated { get; set; }

        public BudgetStatus Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long? FinanceDirectorId { get; set; }

        public long BudgetPeriodId { get; set; }

        [ForeignKey(nameof(BudgetPeriodId))]
        public virtual BudgetPeriod BudgetPeriod { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        public long BudgetCategoryId { get; set; }

        [ForeignKey(nameof(BudgetCategoryId))]
        public virtual BudgetCategory BudgetCategory { get; set; }

        public virtual ICollection<BudgetRequest> BudgetRequests { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }

    }
}