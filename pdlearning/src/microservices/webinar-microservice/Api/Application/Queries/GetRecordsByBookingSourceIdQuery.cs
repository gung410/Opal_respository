using System;
using System.Collections.Generic;
using Microservice.Webinar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries
{
    public class GetRecordsByBookingSourceIdQuery : BaseThunderQuery<List<RecordModel>>
    {
        public Guid SourceId { get; set; }
    }
}
