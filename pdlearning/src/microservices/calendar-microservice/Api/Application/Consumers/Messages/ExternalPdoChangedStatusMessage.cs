#pragma warning disable SA1402 // File may only contain a single type
using Microservice.Calendar.Application.Consumers.Messages.Models;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class ExternalPdoChangedStatusMessage
    {
        public ResultChangeStatus Result { get; set; }

        public AdditionalInformationModel AdditionalInformation { get; set; }
    }

    public class ResultChangeStatus
    {
        public string PdPlanType { get; set; }

        public string PdPlanActivity { get; set; }

        public UserInfoModel ObjectiveInfo { get; set; }

        public ExternalPdoStatusTypeModel TargetStatusType { get; set; }
    }

    public class UserInfoModel
    {
        public UserIdentity Identity { get; set; }
    }

    public class UserIdentity
    {
        /// <summary>
        /// Define string because the value may be integer or GUID.
        /// </summary>
        public string ExtId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
