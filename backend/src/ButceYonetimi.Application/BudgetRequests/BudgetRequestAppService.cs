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
using ButceYonetimi.BudgetRequests.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.StateMachine.Dto;
using ButceYonetimi.BudgetRequestItems.Dto;
using ButceYonetimi.Approvals.Dto;
using ButceYonetimi.Authorization;
using ButceYonetimi.Flows;

namespace ButceYonetimi.BudgetRequests
{
    [Abp.Authorization.AbpAuthorize]
    public class BudgetRequestAppService : AsyncCrudAppService<
        BudgetRequest,
        BudgetRequestDto,
        long,
        PagedBudgetRequestResultRequestDto,
        CreateBudgetRequestDto,
        BudgetRequestDto>,
        IBudgetRequestAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public BudgetRequestAppService(IRepository<BudgetRequest, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.BudgetRequest_Read;
            GetAllPermissionName = PermissionNames.BudgetRequest_Read;
            CreatePermissionName = PermissionNames.BudgetRequest_Create;
            UpdatePermissionName = PermissionNames.BudgetRequest_Update;
            DeletePermissionName = PermissionNames.BudgetRequest_Delete;
        }

        protected override IQueryable<BudgetRequest> CreateFilteredQuery(PagedBudgetRequestResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Budget, x => x.Department)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.RequestNo != null && x.RequestNo.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.Justification != null && x.Justification.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)) ||
                    (x.FinanceApproverRole != null && x.FinanceApproverRole.Contains(input.Keyword)))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.RequestNo.IsNullOrWhiteSpace(), x => x.RequestNo != null && x.RequestNo.Contains(input.RequestNo))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.Justification.IsNullOrWhiteSpace(), x => x.Justification != null && x.Justification.Contains(input.Justification))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(!input.FinanceApproverRole.IsNullOrWhiteSpace(), x => x.FinanceApproverRole != null && x.FinanceApproverRole.Contains(input.FinanceApproverRole))
                .WhereIf(input.TotalAmount.HasValue, x => x.TotalAmount == input.TotalAmount.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (BudgetRequestCurrency)input.Currency.Value)
                .WhereIf(input.TotalAmountTRY.HasValue, x => x.TotalAmountTRY == input.TotalAmountTRY.Value)
                .WhereIf(input.RequestDate.HasValue, x => x.RequestDate == input.RequestDate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (BudgetRequestStatus)input.Status.Value)
                .WhereIf(input.FinanceApprovalThreshold.HasValue, x => x.FinanceApprovalThreshold == input.FinanceApprovalThreshold.Value)
                .WhereIf(input.ManagerApproverId.HasValue, x => x.ManagerApproverId == input.ManagerApproverId.Value)
                .WhereIf(input.TotalAmountFrom.HasValue, x => x.TotalAmount >= input.TotalAmountFrom.Value)
                .WhereIf(input.TotalAmountTo.HasValue, x => x.TotalAmount <= input.TotalAmountTo.Value)
                .WhereIf(input.TotalAmountTRYFrom.HasValue, x => x.TotalAmountTRY >= input.TotalAmountTRYFrom.Value)
                .WhereIf(input.TotalAmountTRYTo.HasValue, x => x.TotalAmountTRY <= input.TotalAmountTRYTo.Value)
                .WhereIf(input.RequestDateFrom.HasValue, x => x.RequestDate >= input.RequestDateFrom.Value)
                .WhereIf(input.RequestDateTo.HasValue, x => x.RequestDate <= input.RequestDateTo.Value)
                .WhereIf(input.FinanceApprovalThresholdFrom.HasValue, x => x.FinanceApprovalThreshold >= input.FinanceApprovalThresholdFrom.Value)
                .WhereIf(input.FinanceApprovalThresholdTo.HasValue, x => x.FinanceApprovalThreshold <= input.FinanceApprovalThresholdTo.Value)
                .WhereIf(input.ManagerApproverIdFrom.HasValue, x => x.ManagerApproverId >= input.ManagerApproverIdFrom.Value)
                .WhereIf(input.ManagerApproverIdTo.HasValue, x => x.ManagerApproverId <= input.ManagerApproverIdTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (BudgetRequestCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (BudgetRequestCurrency)input.CurrencyNot.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (BudgetRequestStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (BudgetRequestStatus)input.StatusNot.Value)
                .WhereIf(input.BudgetId.HasValue, x => x.BudgetId == input.BudgetId.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value);
        }

        public override async Task<BudgetRequestDto> CreateAsync(CreateBudgetRequestDto input)
        {
            // Durum is akisina ait: yeni kayit her zaman baslangic durumunda acilir (gonderme ChangeStatus ile)
            input.Status = (int)BudgetRequestStatus.Draft;
            input.TotalAmount = default; // hesaplanan toplam — alt kayitlardan gelir
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "BudgetRequest", result);
            return result;
        }

        public override async Task<BudgetRequestDto> UpdateAsync(BudgetRequestDto input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingManagerApproval", "PendingFinanceApproval", "Approved", "Cancelled" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.");
            if ((int)existing.Status != input.Status)
                throw new Abp.UI.UserFriendlyException("Durum yalnızca iş akışı eylemleriyle değiştirilebilir.");
            input.TotalAmount = existing.TotalAmount; // hesaplanan toplam formdan degistirilemez
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "BudgetRequest", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingManagerApproval", "PendingFinanceApproval", "Approved", "Cancelled" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; silinemez.");
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "BudgetRequest", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.BudgetRequest_ChangeStatus, PermissionNames.BudgetRequest_Update, RequireAllPermissions = false)]
        public async Task<BudgetRequestDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            // Onay adimlari yalniz atanmis onayci tarafindan, onay servisi uzerinden ilerletilir
            if (input.Action == "Approve")
                throw new Abp.UI.UserFriendlyException("Onay adımları yalnızca onay ekranından ilerletilebilir.");
            // Onay bekleyen kayitta karar (Reject/Revise...) yalniz atanmis onaycinin onay ekranindan verilir; talep sahibi yalniz iptal edebilir
            var current = (await Repository.GetAsync(id)).Status.ToString();
            if (new[] { "PendingManagerApproval", "PendingFinanceApproval" }.Contains(current) && input.Action != "Cancel")
                throw new Abp.UI.UserFriendlyException("Onay bekleyen kayıtta karar yalnızca onay ekranından verilir.");
            return await ApplyStatusChangeAsync(id, input);
        }

        // ABP dinamik API'si attribute'u implementasyonda arar (arayuzdeki yetmez)
        [Abp.Application.Services.RemoteService(false)]
        public async Task<BudgetRequestDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
            => await ApplyStatusChangeAsync(id, input);

        private async Task<BudgetRequestDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "PendingManagerApproval", "Submit", false),
            ("PendingManagerApproval", "PendingFinanceApproval", "Approve", false),
            ("PendingManagerApproval", "Revision", "Revise", false),
            ("PendingManagerApproval", "Rejected", "Reject", false),
            ("PendingFinanceApproval", "Approved", "Approve", false),
            ("PendingFinanceApproval", "Revision", "Revise", false),
            ("PendingFinanceApproval", "Rejected", "Reject", false),
            ("Revision", "PendingManagerApproval", "Resubmit", false),
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
            entity.Status = (BudgetRequestStatus)Enum.Parse(typeof(BudgetRequestStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "BudgetRequest" && a.EntityId == id.ToString() && a.Status == "Pending")
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
                EntityType = "BudgetRequest",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "BudgetRequest", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "BudgetRequest", result);
            return result;
        }


        [Abp.Authorization.AbpAuthorize(PermissionNames.BudgetRequest_Read)]
        public List<GroupCountDto> GetGroupedCount(BudgetRequestGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency", "Status", "BudgetId", "DepartmentId" };
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
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.BudgetRequest_Read)]
        public decimal? GetStats(BudgetRequestStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "RequestDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "TotalAmount", "TotalAmountTRY", "FinanceApprovalThreshold", "ManagerApproverId" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "TotalAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TotalAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TotalAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TotalAmount)
                            : query.Average(x => (decimal?)x.TotalAmount);
                        case "TotalAmountTRY": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TotalAmountTRY)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TotalAmountTRY)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TotalAmountTRY)
                            : query.Average(x => (decimal?)x.TotalAmountTRY);
                        case "FinanceApprovalThreshold": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.FinanceApprovalThreshold)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.FinanceApprovalThreshold)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.FinanceApprovalThreshold)
                            : query.Average(x => (decimal?)x.FinanceApprovalThreshold);
                        case "ManagerApproverId": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ManagerApproverId)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ManagerApproverId)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ManagerApproverId)
                            : query.Average(x => (decimal?)x.ManagerApproverId);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.BudgetRequest_Read)]
        public async Task<BudgetRequestReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.BudgetRequestItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new BudgetRequestReportDto
            {
                Data = ObjectMapper.Map<BudgetRequestDto>(root),
                BudgetRequestItems = ObjectMapper.Map<List<BudgetRequestItemDto>>(
                    root.BudgetRequestItems == null ? new List<BudgetRequestItem>() : root.BudgetRequestItems.ToList()),
                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "BudgetRequest" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
