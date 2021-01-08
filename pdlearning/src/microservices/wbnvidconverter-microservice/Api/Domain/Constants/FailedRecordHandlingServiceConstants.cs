using System.Collections.Generic;
using Microservice.WebinarVideoConverter.Domain.Enums;

namespace Microservice.WebinarVideoConverter.Domain.Constants
{
    public static class FailedRecordHandlingServiceConstants
    {
        public static readonly IDictionary<FailStep, ConvertStatus> _failStepMapping = new Dictionary<FailStep, ConvertStatus>
        {
            { FailStep.Converting, ConvertStatus.New },
            { FailStep.Uploading, ConvertStatus.Converted }
        };
    }
}
