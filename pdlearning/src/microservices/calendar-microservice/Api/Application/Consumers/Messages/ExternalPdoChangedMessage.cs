#pragma warning disable SA1402 // File may only contain a single type
using Microservice.Calendar.Application.Consumers.Messages.Models;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class ExternalPdoChangedMessage
    {
        public ResultExternalPdo Result { get; set; }

        public AdditionalInformationModel AdditionalInformation { get; set; }
    }

    public class ResultExternalPdo
    {
        public ObjectiveInfoModel ObjectiveInfo { get; set; }

        public AnswerModel Answer { get; set; }

        public AssessmentStatusInfoModel AssessmentStatusInfo { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
