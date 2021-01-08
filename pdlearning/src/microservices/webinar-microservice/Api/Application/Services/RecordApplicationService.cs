using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.Queries;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Services
{
    public class RecordApplicationService : ApplicationService, IRecordApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public RecordApplicationService(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public Task<List<RecordModel>> GetRecordsByBookingSourceId(Guid sourceId)
        {
            return _thunderCqrs.SendQuery(new GetRecordsByBookingSourceIdQuery { SourceId = sourceId });
        }
    }
}
