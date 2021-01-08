using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;

namespace Microservice.Webinar.Application.Services
{
    public interface IRecordApplicationService
    {
        Task<List<RecordModel>> GetRecordsByBookingSourceId(Guid sourceId);
    }
}
