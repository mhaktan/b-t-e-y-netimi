using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.StateMachine.Dto;
using ButceYonetimi.Budgets.Dto;

namespace ButceYonetimi.Budgets
{
    public interface IBudgetAppService : IAsyncCrudAppService<
        BudgetDto,
        long,
        PagedBudgetResultRequestDto,
        CreateBudgetDto,
        BudgetDto>
    {
        Task<BudgetDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<BudgetDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(BudgetGroupedCountInput input);
        decimal? GetStats(BudgetStatsInput input);
        Task<BudgetReportDto> GetReportData(long id);
    }
}
