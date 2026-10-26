using System;
using Abp.Application.Services.Dto;

namespace ButceYonetimi.BudgetPeriods.Dto
{
    public class PagedBudgetPeriodResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public int? Year { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }
}
