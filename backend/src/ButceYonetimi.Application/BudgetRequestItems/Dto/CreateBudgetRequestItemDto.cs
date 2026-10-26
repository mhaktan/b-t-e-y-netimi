using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ButceYonetimi.BudgetRequestItems.Dto
{
    [AutoMapTo(typeof(Entities.BudgetRequestItem))]
    public class CreateBudgetRequestItemDto
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

        public long BudgetCategoryId { get; set; }

    }
}