using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.StateMachine.Dto;
using ButceYonetimi.BudgetRequests.Dto;

namespace ButceYonetimi.BudgetRequests
{
    public interface IBudgetRequestAppService : IAsyncCrudAppService<
        BudgetRequestDto,
        long,
        PagedBudgetRequestResultRequestDto,
        CreateBudgetRequestDto,
        BudgetRequestDto>
    {
        Task<BudgetRequestDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<BudgetRequestDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(BudgetRequestGroupedCountInput input);
        decimal? GetStats(BudgetRequestStatsInput input);
        Task<BudgetRequestReportDto> GetReportData(long id);
    }
}
