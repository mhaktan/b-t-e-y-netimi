using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ButceYonetimi.BudgetPeriods.Dto
{
    [AutoMapFrom(typeof(Entities.BudgetPeriod))]
    public class BudgetPeriodDto : EntityDto<long>
    {
        public string Name { get; set; }

        public int Year { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}