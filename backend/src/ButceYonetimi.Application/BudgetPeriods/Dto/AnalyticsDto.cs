using System;
using ButceYonetimi.Analytics.Dto;

namespace ButceYonetimi.BudgetPeriods.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class BudgetPeriodGroupedCountInput : PagedBudgetPeriodResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class BudgetPeriodStatsInput : PagedBudgetPeriodResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
