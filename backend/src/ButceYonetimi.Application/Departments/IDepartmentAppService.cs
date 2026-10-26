using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ButceYonetimi.Analytics.Dto;
using ButceYonetimi.Departments.Dto;

namespace ButceYonetimi.Departments
{
    public interface IDepartmentAppService : IAsyncCrudAppService<
        DepartmentDto,
        long,
        PagedDepartmentResultRequestDto,
        CreateDepartmentDto,
        DepartmentDto>
    {
        Task<DepartmentReportDto> GetReportData(long id);
    }
}
