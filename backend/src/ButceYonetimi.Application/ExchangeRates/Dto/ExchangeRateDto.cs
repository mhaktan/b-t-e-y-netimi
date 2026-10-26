using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ButceYonetimi.ExchangeRates.Dto
{
    [AutoMapFrom(typeof(Entities.ExchangeRate))]
    public class ExchangeRateDto : EntityDto<long>
    {
        public string Name { get; set; }

        public int FromCurrency { get; set; }

        public int ToCurrency { get; set; }

        public decimal Rate { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}