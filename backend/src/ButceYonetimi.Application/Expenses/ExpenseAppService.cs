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
using ButceYonetimi.Expenses.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.StateMachine.Dto;
using ButceYonetimi.Authorization;
using ButceYonetimi.Flows;

namespace ButceYonetimi.Expenses
{
    [Abp.Authorization.AbpAuthorize]
    public class ExpenseAppService : AsyncCrudAppService<
        Expense,
        ExpenseDto,
        long,
        PagedExpenseResultRequestDto,
        CreateExpenseDto,
        ExpenseDto>,
        IExpenseAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public ExpenseAppService(IRepository<Expense, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Expense_Read;
            GetAllPermissionName = PermissionNames.Expense_Read;
            CreatePermissionName = PermissionNames.Expense_Create;
            UpdatePermissionName = PermissionNames.Expense_Update;
            DeletePermissionName = PermissionNames.Expense_Delete;
        }

        protected override IQueryable<Expense> CreateFilteredQuery(PagedExpenseResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Budget, x => x.BudgetCategory)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.ReceiptNo != null && x.ReceiptNo.Contains(input.Keyword)) ||
                    (x.Notes != null && x.Notes.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.ReceiptNo.IsNullOrWhiteSpace(), x => x.ReceiptNo != null && x.ReceiptNo.Contains(input.ReceiptNo))
                .WhereIf(!input.Notes.IsNullOrWhiteSpace(), x => x.Notes != null && x.Notes.Contains(input.Notes))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.Amount.HasValue, x => x.Amount == input.Amount.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (ExpenseCurrency)input.Currency.Value)
                .WhereIf(input.AmountTRY.HasValue, x => x.AmountTRY == input.AmountTRY.Value)
                .WhereIf(input.ExpenseDate.HasValue, x => x.ExpenseDate == input.ExpenseDate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (ExpenseStatus)input.Status.Value)
                .WhereIf(input.ExpenseApproverId.HasValue, x => x.ExpenseApproverId == input.ExpenseApproverId.Value)
                .WhereIf(input.AmountFrom.HasValue, x => x.Amount >= input.AmountFrom.Value)
                .WhereIf(input.AmountTo.HasValue, x => x.Amount <= input.AmountTo.Value)
                .WhereIf(input.AmountTRYFrom.HasValue, x => x.AmountTRY >= input.AmountTRYFrom.Value)
                .WhereIf(input.AmountTRYTo.HasValue, x => x.AmountTRY <= input.AmountTRYTo.Value)
                .WhereIf(input.ExpenseDateFrom.HasValue, x => x.ExpenseDate >= input.ExpenseDateFrom.Value)
                .WhereIf(input.ExpenseDateTo.HasValue, x => x.ExpenseDate <= input.ExpenseDateTo.Value)
                .WhereIf(input.ExpenseApproverIdFrom.HasValue, x => x.ExpenseApproverId >= input.ExpenseApproverIdFrom.Value)
                .WhereIf(input.ExpenseApproverIdTo.HasValue, x => x.ExpenseApproverId <= input.ExpenseApproverIdTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (ExpenseCurrency)input.CurrencyNot.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (ExpenseStatus)input.StatusNot.Value)
                .WhereIf(input.BudgetId.HasValue, x => x.BudgetId == input.BudgetId.Value)
                .WhereIf(input.BudgetCategoryId.HasValue, x => x.BudgetCategoryId == input.BudgetCategoryId.Value);
        }

        public override async Task<ExpenseDto> CreateAsync(CreateExpenseDto input)
        {
            // Durum is akisina ait: yeni kayit her zaman baslangic durumunda acilir (gonderme ChangeStatus ile)
            input.Status = (int)ExpenseStatus.Draft;
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Expense", result);
            return result;
        }

        public override async Task<ExpenseDto> UpdateAsync(ExpenseDto input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Approved", "Rejected" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.");
            if ((int)existing.Status != input.Status)
                throw new Abp.UI.UserFriendlyException("Durum yalnızca iş akışı eylemleriyle değiştirilebilir.");
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Expense", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Approved", "Rejected" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; silinemez.");
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Expense", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.Expense_ChangeStatus, PermissionNames.Expense_Update, RequireAllPermissions = false)]
        public async Task<ExpenseDto> ChangeStatusAsync(long id, ChangeStatusInput input)
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
        public async Task<ExpenseDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
            => await ApplyStatusChangeAsync(id, input);

        private async Task<ExpenseDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "PendingApproval", "Submit", false),
            ("PendingApproval", "Approved", "Approve", true),
            ("PendingApproval", "Rejected", "Reject", true),
            ("PendingApproval", "Draft", "Revise", false)
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
            if (input.Action == "Reject" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Reject requires: revisionNote");
            if (input.Action == "Revise" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Revise requires: revisionNote");
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (ExpenseStatus)Enum.Parse(typeof(ExpenseStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "Expense" && a.EntityId == id.ToString() && a.Status == "Pending")
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
                EntityType = "Expense",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "Expense", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "Expense", result);
            return result;
        }


        [Abp.Authorization.AbpAuthorize(PermissionNames.Expense_Read)]
        public List<GroupCountDto> GetGroupedCount(ExpenseGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency", "Status", "BudgetId", "BudgetCategoryId" };
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
                case "BudgetId":
                    return query
                        .GroupBy(x => new { Key = x.BudgetId, Label = x.Budget == null ? null : x.Budget.Title })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.Expense_Read)]
        public decimal? GetStats(ExpenseStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "ExpenseDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Amount", "AmountTRY", "ExpenseApproverId" };
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
                        case "AmountTRY": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AmountTRY)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AmountTRY)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AmountTRY)
                            : query.Average(x => (decimal?)x.AmountTRY);
                        case "ExpenseApproverId": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ExpenseApproverId)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ExpenseApproverId)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ExpenseApproverId)
                            : query.Average(x => (decimal?)x.ExpenseApproverId);
                        default: return null;
            }
        }

    }
}
