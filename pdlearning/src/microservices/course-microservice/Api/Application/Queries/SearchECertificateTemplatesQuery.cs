using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchECertificateTemplatesQuery : BaseThunderQuery<PagedResultDto<ECertificateTemplateModel>>
    {
        public SearchECertificateType SearchType { get; set; }

        public string SearchText { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
