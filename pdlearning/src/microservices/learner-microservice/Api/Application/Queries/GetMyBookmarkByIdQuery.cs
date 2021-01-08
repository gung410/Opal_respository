using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyBookmarkByIdQuery : BaseThunderQuery<UserBookmarkModel>
    {
        public Guid Id { get; set; }
    }
}
