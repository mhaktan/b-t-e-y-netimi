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
using ButceYonetimi.BudgetCategorys.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.Budgets.Dto;
using ButceYonetimi.BudgetRequestItems.Dto;
using ButceYonetimi.Expenses.Dto;
using ButceYonetimi.Approvals.Dto;
using ButceYonetimi.Authorization;
using ButceYonetimi.Flows;

namespace ButceYonetimi.BudgetCategorys
{
    [Abp.Authorization.AbpAuthorize]
    public class BudgetCategoryAppService : AsyncCrudAppService<
        BudgetCategory,
        BudgetCategoryDto,
        long,
        PagedBudgetCategoryResultRequestDto,
        CreateBudgetCategoryDto,
        BudgetCategoryDto>,
        IBudgetCategoryAppService
    {
        private readonly IFlowEngine _flowEngine;

        public BudgetCategoryAppService(IRepository<BudgetCategory, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.BudgetCategory_Read;
            GetAllPermissionName = PermissionNames.BudgetCategory_Read;
            CreatePermissionName = PermissionNames.BudgetCategory_Create;
            UpdatePermissionName = PermissionNames.BudgetCategory_Update;
            DeletePermissionName = PermissionNames.BudgetCategory_Delete;
        }

        protected override IQueryable<BudgetCategory> CreateFilteredQuery(PagedBudgetCategoryResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description));
        }

        public override async Task<BudgetCategoryDto> CreateAsync(CreateBudgetCategoryDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "BudgetCategory", result);
            return result;
        }

        public override async Task<BudgetCategoryDto> UpdateAsync(BudgetCategoryDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "BudgetCategory", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "BudgetCategory", new { Id = input.Id });
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.BudgetCategory_Read)]
        public async Task<BudgetCategoryReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Budgets)
                .Include(x => x.BudgetRequestItems)
                .Include(x => x.Expenses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new BudgetCategoryReportDto
            {
                Data = ObjectMapper.Map<BudgetCategoryDto>(root),
                Budgets = ObjectMapper.Map<List<BudgetDto>>(
                    root.Budgets == null ? new List<Budget>() : root.Budgets.ToList()),
                BudgetRequestItems = ObjectMapper.Map<List<BudgetRequestItemDto>>(
                    root.BudgetRequestItems == null ? new List<BudgetRequestItem>() : root.BudgetRequestItems.ToList()),
                Expenses = ObjectMapper.Map<List<ExpenseDto>>(
                    root.Expenses == null ? new List<Expense>() : root.Expenses.ToList()),
            };
        }

    }
}
