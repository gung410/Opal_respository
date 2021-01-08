using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetUserSharingByItemIdQuery : BaseThunderQuery<UserSharingModel>
    {
        public Guid ItemId { get; set; }
    }
}
