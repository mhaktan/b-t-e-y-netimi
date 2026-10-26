using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.ExchangeRates.Dto;

namespace ButceYonetimi.ExchangeRates
{
    public interface IExchangeRateAppService : IAsyncCrudAppService<
        ExchangeRateDto,
        long,
        PagedExchangeRateResultRequestDto,
        CreateExchangeRateDto,
        ExchangeRateDto>
    {
        List<GroupCountDto> GetGroupedCount(ExchangeRateGroupedCountInput input);
        decimal? GetStats(ExchangeRateStatsInput input);
    }
}
