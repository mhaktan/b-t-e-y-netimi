using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ButceYonetimi.ExchangeRates.Dto
{
    [AutoMapTo(typeof(Entities.ExchangeRate))]
    public class CreateExchangeRateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int FromCurrency { get; set; }

        public int ToCurrency { get; set; }

        public decimal Rate { get; set; }

        public DateTime EffectiveDate { get; set; }

    }
}