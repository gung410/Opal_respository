using System;
using System.Collections.Generic;
using Microservice.Form.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetPendingApprovalFormsQuery : BaseThunderQuery<PagedResultDto<FormModel>>
    {
        public Guid UserId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
