using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.RequestDtos.RegistrationRequest.ImportParticipant
{
    public class ExportParticipantTemplateRequest
    {
        public ExportParticipantTemplateFileFormat FileFormat { get; set; }
    }
}
