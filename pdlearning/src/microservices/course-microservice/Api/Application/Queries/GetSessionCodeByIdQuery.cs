using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetSessionCodeByIdQuery : BaseThunderQuery<SessionModel>
    {
        public Guid SessionId { get; set; }
    }
}
