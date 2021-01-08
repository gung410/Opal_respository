using System.Collections.Generic;
using Microservice.WebinarVideoConverter.Application.Models;

namespace Microservice.WebinarVideoConverter.Application.Services.Abstractions
{
    public interface IPlaybackScannerService
    {
        List<RecordMetadata> Collect(string playbacksDir);
    }
}
