using Microservice.Course.Application.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchECertificatesRequest : PagedResultRequestDto
    {
        public string SearchText { get; set; }

        public SearchECertificateType SearchType { get; set; }
    }
}
