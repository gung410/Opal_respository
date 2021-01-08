using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetDetailByDigitalContentIdQuery : BaseThunderQuery<DigitalContentModel>
    {
        public Guid DigitalContentId { get; set; }
    }
}
