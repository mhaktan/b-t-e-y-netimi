using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ButceYonetimi.Budgets.Dto
{
    [AutoMapFrom(typeof(Entities.Budget))]
    public class BudgetDto : EntityDto<long>
    {
        public string Title { get; set; }

        public decimal AllocatedAmount { get; set; }

        public int Currency { get; set; }

        public decimal? AllocatedAmountTRY { get; set; }

        public string Description { get; set; }

        public bool? IsAutoCreated { get; set; }

        public int Status { get; set; }

        public string RevisionNote { get; set; }

        public long? FinanceDirectorId { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long BudgetPeriodId { get; set; }

        public string BudgetPeriodDisplay { get; set; }

        public long DepartmentId { get; set; }

        public string DepartmentDisplay { get; set; }

        public long BudgetCategoryId { get; set; }

        public string BudgetCategoryDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}