using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ButceYonetimi.Budgets.Dto
{
    [AutoMapTo(typeof(Entities.Budget))]
    public class CreateBudgetDto
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        public decimal AllocatedAmount { get; set; }

        public int Currency { get; set; }

        public decimal? AllocatedAmountTRY { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public bool? IsAutoCreated { get; set; }

        public int Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long? FinanceDirectorId { get; set; }

        public long BudgetPeriodId { get; set; }

        public long DepartmentId { get; set; }

        public long BudgetCategoryId { get; set; }

    }
}