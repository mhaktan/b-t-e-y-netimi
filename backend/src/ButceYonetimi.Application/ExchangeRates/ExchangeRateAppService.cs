using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ButceYonetimi.Entities;
using ButceYonetimi.ExchangeRates.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.Authorization;
using ButceYonetimi.Flows;

namespace ButceYonetimi.ExchangeRates
{
    [Abp.Authorization.AbpAuthorize]
    public class ExchangeRateAppService : AsyncCrudAppService<
        ExchangeRate,
        ExchangeRateDto,
        long,
        PagedExchangeRateResultRequestDto,
        CreateExchangeRateDto,
        ExchangeRateDto>,
        IExchangeRateAppService
    {
        private readonly IFlowEngine _flowEngine;

        public ExchangeRateAppService(IRepository<ExchangeRate, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.ExchangeRate_Read;
            GetAllPermissionName = PermissionNames.ExchangeRate_Read;
            CreatePermissionName = PermissionNames.ExchangeRate_Create;
            UpdatePermissionName = PermissionNames.ExchangeRate_Update;
            DeletePermissionName = PermissionNames.ExchangeRate_Delete;
        }

        protected override IQueryable<ExchangeRate> CreateFilteredQuery(PagedExchangeRateResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(input.FromCurrency.HasValue, x => x.FromCurrency == (ExchangeRateFromCurrency)input.FromCurrency.Value)
                .WhereIf(input.ToCurrency.HasValue, x => x.ToCurrency == (ExchangeRateToCurrency)input.ToCurrency.Value)
                .WhereIf(input.Rate.HasValue, x => x.Rate == input.Rate.Value)
                .WhereIf(input.EffectiveDate.HasValue, x => x.EffectiveDate == input.EffectiveDate.Value)
                .WhereIf(input.RateFrom.HasValue, x => x.Rate >= input.RateFrom.Value)
                .WhereIf(input.RateTo.HasValue, x => x.Rate <= input.RateTo.Value)
                .WhereIf(input.EffectiveDateFrom.HasValue, x => x.EffectiveDate >= input.EffectiveDateFrom.Value)
                .WhereIf(input.EffectiveDateTo.HasValue, x => x.EffectiveDate <= input.EffectiveDateTo.Value)
                .WhereIf(!input.FromCurrencyIn.IsNullOrWhiteSpace(), x => input.FromCurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExchangeRateFromCurrency?)int.Parse(v.Trim()))
                    .Contains(x.FromCurrency))
                .WhereIf(input.FromCurrencyNot.HasValue, x => x.FromCurrency != (ExchangeRateFromCurrency)input.FromCurrencyNot.Value)
                .WhereIf(!input.ToCurrencyIn.IsNullOrWhiteSpace(), x => input.ToCurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExchangeRateToCurrency?)int.Parse(v.Trim()))
                    .Contains(x.ToCurrency))
                .WhereIf(input.ToCurrencyNot.HasValue, x => x.ToCurrency != (ExchangeRateToCurrency)input.ToCurrencyNot.Value);
        }

        public override async Task<ExchangeRateDto> CreateAsync(CreateExchangeRateDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "ExchangeRate", result);
            return result;
        }

        public override async Task<ExchangeRateDto> UpdateAsync(ExchangeRateDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "ExchangeRate", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "ExchangeRate", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExchangeRate_Read)]
        public List<GroupCountDto> GetGroupedCount(ExchangeRateGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "FromCurrency", "ToCurrency" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "FromCurrency":
                    return query
                        .GroupBy(x => x.FromCurrency)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "ToCurrency":
                    return query
                        .GroupBy(x => x.ToCurrency)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.ExchangeRate_Read)]
        public decimal? GetStats(ExchangeRateStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "EffectiveDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Rate" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Rate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Rate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Rate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Rate)
                            : query.Average(x => (decimal?)x.Rate);
                        default: return null;
            }
        }

    }
}
