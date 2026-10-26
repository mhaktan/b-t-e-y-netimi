using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ButceYonetimi.Entities
{
    [Table("BudgetPeriods")]
    public class BudgetPeriod : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int Year { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; }

    }
}