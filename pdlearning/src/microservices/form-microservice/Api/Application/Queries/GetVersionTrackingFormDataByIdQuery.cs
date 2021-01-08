using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetVersionTrackingFormDataByIdQuery : BaseThunderQuery<VersionTrackingFormDataModel>
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
