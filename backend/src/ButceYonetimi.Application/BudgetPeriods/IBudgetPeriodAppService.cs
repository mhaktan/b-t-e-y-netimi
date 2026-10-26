using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.BudgetPeriods.Dto;

namespace ButceYonetimi.BudgetPeriods
{
    public interface IBudgetPeriodAppService : IAsyncCrudAppService<
        BudgetPeriodDto,
        long,
        PagedBudgetPeriodResultRequestDto,
        CreateBudgetPeriodDto,
        BudgetPeriodDto>
    {
        decimal? GetStats(BudgetPeriodStatsInput input);
        Task<BudgetPeriodReportDto> GetReportData(long id);
    }
}
