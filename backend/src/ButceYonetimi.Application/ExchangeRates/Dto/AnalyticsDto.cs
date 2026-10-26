using System;
using ButceYonetimi.Analytics.Dto;

namespace ButceYonetimi.ExchangeRates.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class ExchangeRateGroupedCountInput : PagedExchangeRateResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class ExchangeRateStatsInput : PagedExchangeRateResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
