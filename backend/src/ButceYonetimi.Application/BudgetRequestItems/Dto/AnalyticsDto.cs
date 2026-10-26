using System;
using ButceYonetimi.Analytics.Dto;

namespace ButceYonetimi.BudgetRequestItems.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class BudgetRequestItemGroupedCountInput : PagedBudgetRequestItemResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class BudgetRequestItemStatsInput : PagedBudgetRequestItemResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
