using System;
using System.Collections.Generic;
using ButceYonetimi.BudgetRequests.Dto;
using ButceYonetimi.Expenses.Dto;
using ButceYonetimi.Approvals.Dto;

namespace ButceYonetimi.Budgets.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class BudgetReportDto
    {
        public BudgetDto Data { get; set; }
        public List<BudgetRequestDto> BudgetRequests { get; set; }
        public List<ExpenseDto> Expenses { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
