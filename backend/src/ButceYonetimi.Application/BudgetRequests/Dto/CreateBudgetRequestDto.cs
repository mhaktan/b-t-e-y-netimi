using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ButceYonetimi.BudgetRequests.Dto
{
    [AutoMapTo(typeof(Entities.BudgetRequest))]
    public class CreateBudgetRequestDto
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

        public int Currency { get; set; }

        public decimal? TotalAmountTRY { get; set; }

        public DateTime RequestDate { get; set; }

        public int Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public decimal? FinanceApprovalThreshold { get; set; }

        public long? ManagerApproverId { get; set; }

        public string FinanceApproverRole { get; set; }

        public long BudgetId { get; set; }

        public long DepartmentId { get; set; }

    }
}