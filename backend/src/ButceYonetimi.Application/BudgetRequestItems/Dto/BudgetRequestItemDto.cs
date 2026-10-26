using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ButceYonetimi.BudgetRequestItems.Dto
{
    [AutoMapFrom(typeof(Entities.BudgetRequestItem))]
    public class BudgetRequestItemDto : EntityDto<long>
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime? PlannedDate { get; set; }

        public string Notes { get; set; }

        public long BudgetRequestId { get; set; }

        public string BudgetRequestDisplay { get; set; }

        public long BudgetCategoryId { get; set; }

        public string BudgetCategoryDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}