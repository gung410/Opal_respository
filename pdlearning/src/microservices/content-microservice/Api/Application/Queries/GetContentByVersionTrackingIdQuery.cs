using System;
using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetContentByVersionTrackingIdQuery : BaseThunderQuery<DigitalContentModel>
    {
        public Guid VersionTrackingId { get; set; }

        public Guid UserId { get; set; }
    }
}
