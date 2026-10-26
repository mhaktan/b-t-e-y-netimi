using AutoMapper;
using ButceYonetimi.Approvals.Dto;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Approvals
{
    public class ApprovalMapProfile : Profile
    {
        public ApprovalMapProfile()
        {
            CreateMap<ApprovalRecord, ApprovalRecordDto>();
            CreateMap<StatusChangeLog, StatusChangeLogDto>();
        }
    }
}
