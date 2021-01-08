using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Queries;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetMyCertificatesRequest : PagedResultRequestDto
    {
        public GetMyCertificatesQuery ToQuery()
        {
            return new GetMyCertificatesQuery
            {
                PageInfo = new PagedResultRequestDto { MaxResultCount = MaxResultCount, SkipCount = SkipCount }
            };
        }
    }
}
