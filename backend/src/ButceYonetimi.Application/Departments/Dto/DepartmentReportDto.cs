using System;
using System.Collections.Generic;
using ButceYonetimi.Budgets.Dto;
using ButceYonetimi.BudgetRequests.Dto;

namespace ButceYonetimi.Departments.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class DepartmentReportDto
    {
        public DepartmentDto Data { get; set; }
        public List<BudgetDto> Budgets { get; set; }
        public List<BudgetRequestDto> BudgetRequests { get; set; }
    }
}
