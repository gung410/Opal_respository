using System.Collections.Generic;
using Microservice.WebinarVideoConverter.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Queries
{
    public class GetCanUploadRecordQuery : BaseThunderQuery<List<ConvertingTrackingModel>>
    {
        public int MaxConcurrentUploads { get; set; }
    }
}
