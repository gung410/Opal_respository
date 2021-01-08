using System;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.PDPM.Messages
{
    public class PDPM_PDPlanChangeStatusMessage
    {
        public PDPM_PDPlanChangeStatusMessageResult Result { get; set; }

        public AdditionalInformation AdditionalInformation { get; set; }
    }

    public class PDPM_PDPlanChangeStatusMessageResult
    {
        public Guid ResultExtId { get; set; }

        public long TargetResultId { get; set; }

        public string PdPlanType { get; set; }

        public AnalyticPdPlanActivity PdPlanActivity { get; set; }

        public SourceStatusType SourceStatusType { get; set; }

        public TargetStatusType TargetStatusType { get; set; }
    }

    public class SourceStatusType
    {
        public int StatusTypeId { get; set; }

        public string StatusTypeCode { get; set; }
    }

    public class TargetStatusType
    {
        public int StatusTypeId { get; set; }

        public string StatusTypeCode { get; set; }
    }

    public class AdditionalInformation
    {
        public Guid UpdatedBy { get; set; }
    }
}
