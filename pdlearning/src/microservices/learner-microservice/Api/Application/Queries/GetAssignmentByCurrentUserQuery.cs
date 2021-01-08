using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetAssignmentByCurrentUserQuery : BaseThunderQuery<PagedResultDto<MyAssignmentModel>>, IPagedResultAware
    {
        public Guid RegistrationId { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
