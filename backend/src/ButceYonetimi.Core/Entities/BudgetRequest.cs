using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ButceYonetimi.Entities
{
    // State Machine: status — Draft → PendingManagerApproval → PendingFinanceApproval → Approved → Revision → Rejected → Cancelled
    // Initial: Draft | Transitions: Draft→PendingManagerApproval[Submit], PendingManagerApproval→PendingFinanceApproval[Approve], PendingManagerApproval→Revision[Revise], PendingManagerApproval→Rejected[Reject], PendingFinanceApproval→Approved[Approve], PendingFinanceApproval→Revision[Revise], PendingFinanceApproval→Rejected[Reject], Revision→PendingManagerApproval[Resubmit], *→Cancelled[Cancel]
    [Table("BudgetRequests")]
    public class BudgetRequest : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        [MaxLength(50)]
        public string RequestNo { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Justification { get; set; }

        public decimal? TotalAmount { get; set; }

        public BudgetRequestCurrency Currency { get; set; }

        public decimal? TotalAmountTRY { get; set; }

        public DateTime RequestDate { get; set; }

        public BudgetRequestStatus Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public decimal? FinanceApprovalThreshold { get; set; }

        public long? ManagerApproverId { get; set; }

        public string FinanceApproverRole { get; set; }

        public long BudgetId { get; set; }

        [ForeignKey(nameof(BudgetId))]
        public virtual Budget Budget { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        public virtual ICollection<BudgetRequestItem> BudgetRequestItems { get; set; }

    }
}