using System;
using Abp.Application.Services.Dto;

namespace ButceYonetimi.BudgetCategorys.Dto
{
    public class PagedBudgetCategoryResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
