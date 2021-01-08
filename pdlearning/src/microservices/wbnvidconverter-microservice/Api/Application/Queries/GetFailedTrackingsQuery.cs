using System.Collections.Generic;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Queries
{
    public class GetFailedTrackingsQuery : BaseThunderQuery<List<ConvertingTracking>>
    {
    }
}
