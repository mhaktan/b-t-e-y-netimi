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
using ButceYonetimi.Budgets.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.StateMachine.Dto;
using ButceYonetimi.BudgetRequests.Dto;
using ButceYonetimi.Expenses.Dto;
using ButceYonetimi.Approvals.Dto;
using ButceYonetimi.Authorization;
using ButceYonetimi.Flows;

namespace ButceYonetimi.Budgets
{
    [Abp.Authorization.AbpAuthorize]
    public class BudgetAppService : AsyncCrudAppService<
        Budget,
        BudgetDto,
        long,
        PagedBudgetResultRequestDto,
        CreateBudgetDto,
        BudgetDto>,
        IBudgetAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public BudgetAppService(IRepository<Budget, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Budget_Read;
            GetAllPermissionName = PermissionNames.Budget_Read;
            CreatePermissionName = PermissionNames.Budget_Create;
            UpdatePermissionName = PermissionNames.Budget_Update;
            DeletePermissionName = PermissionNames.Budget_Delete;
        }

        protected override IQueryable<Budget> CreateFilteredQuery(PagedBudgetResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.BudgetPeriod, x => x.Department, x => x.BudgetCategory)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.AllocatedAmount.HasValue, x => x.AllocatedAmount == input.AllocatedAmount.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (BudgetCurrency)input.Currency.Value)
                .WhereIf(input.AllocatedAmountTRY.HasValue, x => x.AllocatedAmountTRY == input.AllocatedAmountTRY.Value)
                .WhereIf(input.IsAutoCreated.HasValue, x => x.IsAutoCreated == input.IsAutoCreated.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (BudgetStatus)input.Status.Value)
                .WhereIf(input.FinanceDirectorId.HasValue, x => x.FinanceDirectorId == input.FinanceDirectorId.Value)
                .WhereIf(input.AllocatedAmountFrom.HasValue, x => x.AllocatedAmount >= input.AllocatedAmountFrom.Value)
                .WhereIf(input.AllocatedAmountTo.HasValue, x => x.AllocatedAmount <= input.AllocatedAmountTo.Value)
                .WhereIf(input.AllocatedAmountTRYFrom.HasValue, x => x.AllocatedAmountTRY >= input.AllocatedAmountTRYFrom.Value)
                .WhereIf(input.AllocatedAmountTRYTo.HasValue, x => x.AllocatedAmountTRY <= input.AllocatedAmountTRYTo.Value)
                .WhereIf(input.FinanceDirectorIdFrom.HasValue, x => x.FinanceDirectorId >= input.FinanceDirectorIdFrom.Value)
                .WhereIf(input.FinanceDirectorIdTo.HasValue, x => x.FinanceDirectorId <= input.FinanceDirectorIdTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (BudgetCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (BudgetCurrency)input.CurrencyNot.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (BudgetStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (BudgetStatus)input.StatusNot.Value)
                .WhereIf(input.BudgetPeriodId.HasValue, x => x.BudgetPeriodId == input.BudgetPeriodId.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value)
                .WhereIf(input.BudgetCategoryId.HasValue, x => x.BudgetCategoryId == input.BudgetCategoryId.Value);
        }

        public override async Task<BudgetDto> CreateAsync(CreateBudgetDto input)
        {
            // Durum is akisina ait: yeni kayit her zaman baslangic durumunda acilir (gonderme ChangeStatus ile)
            input.Status = (int)BudgetStatus.Draft;
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Budget", result);
            return result;
        }

        public override async Task<BudgetDto> UpdateAsync(BudgetDto input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Active", "Cancelled" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.");
            if ((int)existing.Status != input.Status)
                throw new Abp.UI.UserFriendlyException("Durum yalnızca iş akışı eylemleriyle değiştirilebilir.");
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Budget", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Active", "Cancelled" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; silinemez.");
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Budget", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.Budget_ChangeStatus, PermissionNames.Budget_Update, RequireAllPermissions = false)]
        public async Task<BudgetDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            // Onay adimlari yalniz atanmis onayci tarafindan, onay servisi uzerinden ilerletilir
            if (input.Action == "Approve")
                throw new Abp.UI.UserFriendlyException("Onay adımları yalnızca onay ekranından ilerletilebilir.");
            // Onay bekleyen kayitta karar (Reject/Revise...) yalniz atanmis onaycinin onay ekranindan verilir; talep sahibi yalniz iptal edebilir
            var current = (await Repository.GetAsync(id)).Status.ToString();
            if (new[] { "PendingApproval" }.Contains(current) && input.Action != "Cancel")
                throw new Abp.UI.UserFriendlyException("Onay bekleyen kayıtta karar yalnızca onay ekranından verilir.");
            return await ApplyStatusChangeAsync(id, input);
        }

        // ABP dinamik API'si attribute'u implementasyonda arar (arayuzdeki yetmez)
        [Abp.Application.Services.RemoteService(false)]
        public async Task<BudgetDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
            => await ApplyStatusChangeAsync(id, input);

        private async Task<BudgetDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "PendingApproval", "Submit", false),
            ("PendingApproval", "Active", "Approve", false),
            ("PendingApproval", "Draft", "Revise", false),
            ("Active", "Closed", "Close", false),
            ("*", "Cancelled", "Cancel", true)
            };

            var transition = transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Onay ekranindan gelen serbest metni gecisin zorunlu alanlarina tasi
            if (input.ActionData == null) input.ActionData = new Dictionary<string, string>();
            var genericNote = input.ActionData.ContainsKey("comment") && !string.IsNullOrWhiteSpace(input.ActionData["comment"])
                ? input.ActionData["comment"]
                : (input.ActionData.ContainsKey("revisionNote") ? input.ActionData["revisionNote"] : null);
            if (!string.IsNullOrWhiteSpace(genericNote))
            {
                foreach (var rf in new[] { "revisionNote" })
                {
                    if (!input.ActionData.ContainsKey(rf) || string.IsNullOrWhiteSpace(input.ActionData[rf]))
                        input.ActionData[rf] = genericNote;
                }
            }
            // Validate required fields per transition
            if (input.Action == "Revise" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Revise requires: revisionNote");
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (BudgetStatus)Enum.Parse(typeof(BudgetStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "Budget" && a.EntityId == id.ToString() && a.Status == "Pending")
                    .ToList();
                foreach (var pendingRec in pending)
                {
                    pendingRec.Status = "Cancelled";
                    pendingRec.ActionTaken = "Cancel";
                    pendingRec.ActionDate = DateTime.UtcNow;
                    pendingRec.Comment = "Entity cancelled by submitter.";
                    await _approvalRepo.UpdateAsync(pendingRec);
                }
            }

            // Log status change
            await _statusChangeLogRepo.InsertAsync(new Entities.StatusChangeLog
            {
                EntityType = "Budget",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "Budget", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "Budget", result);
            return result;
        }


        [Abp.Authorization.AbpAuthorize(PermissionNames.Budget_Read)]
        public List<GroupCountDto> GetGroupedCount(BudgetGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency", "Status", "BudgetPeriodId", "DepartmentId", "BudgetCategoryId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "Currency":
                    return query
                        .GroupBy(x => x.Currency)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "Status":
                    return query
                        .GroupBy(x => x.Status)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "BudgetPeriodId":
                    return query
                        .GroupBy(x => new { Key = x.BudgetPeriodId, Label = x.BudgetPeriod == null ? null : x.BudgetPeriod.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "DepartmentId":
                    return query
                        .GroupBy(x => new { Key = x.DepartmentId, Label = x.Department == null ? null : x.Department.Name })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.Budget_Read)]
        public decimal? GetStats(BudgetStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new string[0];
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "AllocatedAmount", "AllocatedAmountTRY", "FinanceDirectorId" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "AllocatedAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AllocatedAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AllocatedAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AllocatedAmount)
                            : query.Average(x => (decimal?)x.AllocatedAmount);
                        case "AllocatedAmountTRY": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AllocatedAmountTRY)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AllocatedAmountTRY)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AllocatedAmountTRY)
                            : query.Average(x => (decimal?)x.AllocatedAmountTRY);
                        case "FinanceDirectorId": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.FinanceDirectorId)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.FinanceDirectorId)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.FinanceDirectorId)
                            : query.Average(x => (decimal?)x.FinanceDirectorId);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Budget_Read)]
        public async Task<BudgetReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.BudgetRequests)
                .Include(x => x.Expenses)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new BudgetReportDto
            {
                Data = ObjectMapper.Map<BudgetDto>(root),
                BudgetRequests = ObjectMapper.Map<List<BudgetRequestDto>>(
                    root.BudgetRequests == null ? new List<BudgetRequest>() : root.BudgetRequests.ToList()),
                Expenses = ObjectMapper.Map<List<ExpenseDto>>(
                    root.Expenses == null ? new List<Expense>() : root.Expenses.ToList()),
                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "Budget" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
