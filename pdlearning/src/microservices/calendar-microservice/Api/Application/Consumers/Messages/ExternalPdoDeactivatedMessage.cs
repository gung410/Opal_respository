#pragma warning disable SA1402 // File may only contain a single type
using Microservice.Calendar.Application.Consumers.Messages.Models;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class ExternalPdoDeactivatedMessage
    {
        public ResultExternalPdoDeactivated Result { get; set; }

        public AdditionalInformationModel AdditionalInformation { get; set; }
    }

    public class ResultExternalPdoDeactivated
    {
        public ObjectiveInfoModel ObjectiveInfo { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
