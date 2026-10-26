using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.BudgetCategorys.Dto;

namespace ButceYonetimi.BudgetCategorys
{
    public interface IBudgetCategoryAppService : IAsyncCrudAppService<
        BudgetCategoryDto,
        long,
        PagedBudgetCategoryResultRequestDto,
        CreateBudgetCategoryDto,
        BudgetCategoryDto>
    {
        Task<BudgetCategoryReportDto> GetReportData(long id);
    }
}
