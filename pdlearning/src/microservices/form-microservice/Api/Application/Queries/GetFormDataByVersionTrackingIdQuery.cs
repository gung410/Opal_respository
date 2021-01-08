using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetFormDataByVersionTrackingIdQuery : BaseThunderQuery<VersionTrackingFormDataModel>
    {
        public Guid VersionTrackingId { get; set; }

        public Guid UserId { get; set; }
    }
}
