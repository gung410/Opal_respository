using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetUserSharingByItemIdsQuery : BaseThunderQuery<List<UserSharingModel>>
    {
        public Guid[] ItemIds { get; set; }
    }
}
