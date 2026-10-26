using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.BudgetRequestItems.Dto;

namespace ButceYonetimi.BudgetRequestItems
{
    public interface IBudgetRequestItemAppService : IAsyncCrudAppService<
        BudgetRequestItemDto,
        long,
        PagedBudgetRequestItemResultRequestDto,
        CreateBudgetRequestItemDto,
        BudgetRequestItemDto>
    {
        List<GroupCountDto> GetGroupedCount(BudgetRequestItemGroupedCountInput input);
        decimal? GetStats(BudgetRequestItemStatsInput input);
    }
}
