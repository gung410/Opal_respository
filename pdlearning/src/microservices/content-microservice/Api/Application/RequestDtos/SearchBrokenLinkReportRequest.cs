using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.RequestDtos
{
    public class SearchBrokenLinkReportRequest
    {
        public Guid OriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
