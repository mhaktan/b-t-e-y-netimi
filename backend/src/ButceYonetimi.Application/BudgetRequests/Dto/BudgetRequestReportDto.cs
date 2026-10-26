using System;
using System.Collections.Generic;
using ButceYonetimi.BudgetRequestItems.Dto;
using ButceYonetimi.Approvals.Dto;

namespace ButceYonetimi.BudgetRequests.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class BudgetRequestReportDto
    {
        public BudgetRequestDto Data { get; set; }
        public List<BudgetRequestItemDto> BudgetRequestItems { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
