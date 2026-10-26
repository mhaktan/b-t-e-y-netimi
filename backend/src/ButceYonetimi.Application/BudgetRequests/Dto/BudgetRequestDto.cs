using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ButceYonetimi.BudgetRequests.Dto
{
    [AutoMapFrom(typeof(Entities.BudgetRequest))]
    public class BudgetRequestDto : EntityDto<long>
    {
        public string Title { get; set; }

        public string RequestNo { get; set; }

        public string Description { get; set; }

        public string Justification { get; set; }

        public decimal? TotalAmount { get; set; }

        public int Currency { get; set; }

        public decimal? TotalAmountTRY { get; set; }

        public DateTime RequestDate { get; set; }

        public int Status { get; set; }

        public string RevisionNote { get; set; }

        public decimal? FinanceApprovalThreshold { get; set; }

        public long? ManagerApproverId { get; set; }

        public string FinanceApproverRole { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long BudgetId { get; set; }

        public string BudgetDisplay { get; set; }

        public long DepartmentId { get; set; }

        public string DepartmentDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}