using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ButceYonetimi.BudgetCategorys.Dto
{
    [AutoMapTo(typeof(Entities.BudgetCategory))]
    public class CreateBudgetCategoryDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

    }
}