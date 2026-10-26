using System;
using Abp.Application.Services.Dto;

namespace ButceYonetimi.ExchangeRates.Dto
{
    public class PagedExchangeRateResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public int? FromCurrency { get; set; }
        public int? ToCurrency { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public decimal? RateFrom { get; set; }
        public decimal? RateTo { get; set; }
        public DateTime? EffectiveDateFrom { get; set; }
        public DateTime? EffectiveDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string FromCurrencyIn { get; set; }
        public int? FromCurrencyNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string ToCurrencyIn { get; set; }
        public int? ToCurrencyNot { get; set; }
    }
}
