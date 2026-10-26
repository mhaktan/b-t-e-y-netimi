using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ButceYonetimi.Entities
{
    [Table("BudgetRequestItems")]
    public class BudgetRequestItem : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime? PlannedDate { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        public long BudgetRequestId { get; set; }

        [ForeignKey(nameof(BudgetRequestId))]
        public virtual BudgetRequest BudgetRequest { get; set; }

        public long BudgetCategoryId { get; set; }

        [ForeignKey(nameof(BudgetCategoryId))]
        public virtual BudgetCategory BudgetCategory { get; set; }

    }
}