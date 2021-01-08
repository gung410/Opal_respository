using Microservice.Calendar.Application.Consumers.Messages.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages.Helpers
{
    public class ExternalPdoUserEventAcceptedMapper
    {
        public static bool GetExternalPdoUserEventAccepted(AssessmentStatus externalPdoStatus)
        {
            return externalPdoStatus == AssessmentStatus.Started || externalPdoStatus == AssessmentStatus.Approved;
        }
    }
}
