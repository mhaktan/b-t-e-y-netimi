using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ButceYonetimi.Entities
{
    [Table("ExchangeRates")]
    public class ExchangeRate : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ExchangeRateFromCurrency FromCurrency { get; set; }

        public ExchangeRateToCurrency ToCurrency { get; set; }

        public decimal Rate { get; set; }

        public DateTime EffectiveDate { get; set; }

    }
}