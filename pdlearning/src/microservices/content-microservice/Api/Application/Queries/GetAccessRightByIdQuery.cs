using System;
using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetAccessRightByIdQuery : BaseThunderQuery<AccessRightModel>
    {
        public Guid Id { get; set; }
    }
}
