using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.StateMachine.Dto;
using ButceYonetimi.Expenses.Dto;

namespace ButceYonetimi.Expenses
{
    public interface IExpenseAppService : IAsyncCrudAppService<
        ExpenseDto,
        long,
        PagedExpenseResultRequestDto,
        CreateExpenseDto,
        ExpenseDto>
    {
        Task<ExpenseDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<ExpenseDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(ExpenseGroupedCountInput input);
        decimal? GetStats(ExpenseStatsInput input);
    }
}
