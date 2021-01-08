using System.Collections.Generic;
using Microservice.WebinarVideoConverter.Application.Models;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Queries
{
    public class GetConvertingTrackingsByStatusQuery : BaseThunderQuery<List<ConvertingTrackingModel>>
    {
        public ConvertStatus Status { get; set; }
    }
}
