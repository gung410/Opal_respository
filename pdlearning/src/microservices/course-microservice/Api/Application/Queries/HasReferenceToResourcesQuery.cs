using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class HasReferenceToResourceQuery : BaseThunderQuery<bool>
    {
        public Guid ResourceId { get; set; }
    }
}
