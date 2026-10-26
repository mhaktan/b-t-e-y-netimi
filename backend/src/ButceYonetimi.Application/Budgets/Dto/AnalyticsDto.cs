using System;
using ButceYonetimi.Analytics.Dto;

namespace ButceYonetimi.Budgets.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class BudgetGroupedCountInput : PagedBudgetResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class BudgetStatsInput : PagedBudgetResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
