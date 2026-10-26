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
using ButceYonetimi.BudgetRequestItems.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.Authorization;
using ButceYonetimi.Flows;

namespace ButceYonetimi.BudgetRequestItems
{
    [Abp.Authorization.AbpAuthorize]
    public class BudgetRequestItemAppService : AsyncCrudAppService<
        BudgetRequestItem,
        BudgetRequestItemDto,
        long,
        PagedBudgetRequestItemResultRequestDto,
        CreateBudgetRequestItemDto,
        BudgetRequestItemDto>,
        IBudgetRequestItemAppService
    {
        private readonly IFlowEngine _flowEngine;

        public BudgetRequestItemAppService(IRepository<BudgetRequestItem, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.BudgetRequestItem_Read;
            GetAllPermissionName = PermissionNames.BudgetRequestItem_Read;
            CreatePermissionName = PermissionNames.BudgetRequestItem_Create;
            UpdatePermissionName = PermissionNames.BudgetRequestItem_Update;
            DeletePermissionName = PermissionNames.BudgetRequestItem_Delete;
        }

        protected override IQueryable<BudgetRequestItem> CreateFilteredQuery(PagedBudgetRequestItemResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.BudgetRequest, x => x.BudgetCategory)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.Notes != null && x.Notes.Contains(input.Keyword)))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.Notes.IsNullOrWhiteSpace(), x => x.Notes != null && x.Notes.Contains(input.Notes))
                .WhereIf(input.Amount.HasValue, x => x.Amount == input.Amount.Value)
                .WhereIf(input.Quantity.HasValue, x => x.Quantity == input.Quantity.Value)
                .WhereIf(input.UnitPrice.HasValue, x => x.UnitPrice == input.UnitPrice.Value)
                .WhereIf(input.PlannedDate.HasValue, x => x.PlannedDate == input.PlannedDate.Value)
                .WhereIf(input.AmountFrom.HasValue, x => x.Amount >= input.AmountFrom.Value)
                .WhereIf(input.AmountTo.HasValue, x => x.Amount <= input.AmountTo.Value)
                .WhereIf(input.QuantityFrom.HasValue, x => x.Quantity >= input.QuantityFrom.Value)
                .WhereIf(input.QuantityTo.HasValue, x => x.Quantity <= input.QuantityTo.Value)
                .WhereIf(input.UnitPriceFrom.HasValue, x => x.UnitPrice >= input.UnitPriceFrom.Value)
                .WhereIf(input.UnitPriceTo.HasValue, x => x.UnitPrice <= input.UnitPriceTo.Value)
                .WhereIf(input.PlannedDateFrom.HasValue, x => x.PlannedDate >= input.PlannedDateFrom.Value)
                .WhereIf(input.PlannedDateTo.HasValue, x => x.PlannedDate <= input.PlannedDateTo.Value)
                .WhereIf(input.BudgetRequestId.HasValue, x => x.BudgetRequestId == input.BudgetRequestId.Value)
                .WhereIf(input.BudgetCategoryId.HasValue, x => x.BudgetCategoryId == input.BudgetCategoryId.Value);
        }

        public override async Task<BudgetRequestItemDto> CreateAsync(CreateBudgetRequestItemDto input)
        {
            var result = await base.CreateAsync(input);
            await CurrentUnitOfWork.SaveChangesAsync();
            await RecalcTotalsAsync((object)result.BudgetRequestId);
            await _flowEngine.TriggerAsync("on-create", "BudgetRequestItem", result);
            return result;
        }

        public override async Task<BudgetRequestItemDto> UpdateAsync(BudgetRequestItemDto input)
        {
            var existing = await Repository.GetAsync(input.Id);
            var oldParents = new object[] { (object)existing.BudgetRequestId };
            var result = await base.UpdateAsync(input);
            await CurrentUnitOfWork.SaveChangesAsync();
            await RecalcTotalsAsync(oldParents.Concat(new object[] { (object)result.BudgetRequestId }).ToArray());
            await _flowEngine.TriggerAsync("on-update", "BudgetRequestItem", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var existing = await Repository.GetAsync(input.Id);
            var oldParents = new object[] { (object)existing.BudgetRequestId };
            await base.DeleteAsync(input);
            await CurrentUnitOfWork.SaveChangesAsync();
            await RecalcTotalsAsync(oldParents);
            await _flowEngine.TriggerAsync("on-delete", "BudgetRequestItem", new { Id = input.Id });
        }

        private async Task RecalcTotalsAsync(params object[] parentIds)
        {
            foreach (var raw in parentIds.Where(x => x != null).Distinct())
            {
                {
                    var repo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<BudgetRequest, long>>();
                    var pid = (long)Convert.ChangeType(raw, typeof(long));
                    var parent = await repo.FirstOrDefaultAsync(pid);
                    if (parent != null)
                    {
                        parent.TotalAmount = (decimal)(Repository.GetAll().Where(c => c.BudgetRequestId == pid).Sum(c => (decimal?)c.Amount) ?? 0);
                        await repo.UpdateAsync(parent);
                    }
                }
            }
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.BudgetRequestItem_Read)]
        public List<GroupCountDto> GetGroupedCount(BudgetRequestItemGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "BudgetRequestId", "BudgetCategoryId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "BudgetRequestId":
                    return query
                        .GroupBy(x => new { Key = x.BudgetRequestId, Label = x.BudgetRequest == null ? null : x.BudgetRequest.Title })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "BudgetCategoryId":
                    return query
                        .GroupBy(x => new { Key = x.BudgetCategoryId, Label = x.BudgetCategory == null ? null : x.BudgetCategory.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.BudgetRequestItem_Read)]
        public decimal? GetStats(BudgetRequestItemStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "PlannedDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Amount", "Quantity", "UnitPrice" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Amount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Amount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Amount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Amount)
                            : query.Average(x => (decimal?)x.Amount);
                        case "Quantity": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Quantity)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Quantity)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Quantity)
                            : query.Average(x => (decimal?)x.Quantity);
                        case "UnitPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.UnitPrice)
                            : query.Average(x => (decimal?)x.UnitPrice);
                        default: return null;
            }
        }

    }
}
