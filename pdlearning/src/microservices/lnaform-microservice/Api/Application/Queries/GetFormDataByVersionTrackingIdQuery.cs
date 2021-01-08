using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormDataByVersionTrackingIdQuery : BaseThunderQuery<VersionTrackingFormDataModel>
    {
        public Guid VersionTrackingId { get; set; }

        public Guid UserId { get; set; }
    }
}
