using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetMyCertificatesQuery : BaseThunderQuery<PagedResultDto<RegistrationModel>>
    {
        public PagedResultRequestDto PageInfo { get; set; }
    }
}
