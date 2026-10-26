using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ButceYonetimi.Departments.Dto
{
    [AutoMapTo(typeof(Entities.Department))]
    public class CreateDepartmentDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

    }
}