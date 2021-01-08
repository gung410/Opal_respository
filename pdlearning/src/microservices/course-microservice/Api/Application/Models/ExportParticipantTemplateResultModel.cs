using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.Models
{
#pragma warning disable CA1024 // Use properties where appropriate
    public class ExportParticipantTemplateResultModel
    {
        public byte[] FileContent { get; set; }

        public ExportParticipantTemplateFileFormat FileFormat { get; set; }

        public string GetFileName()
        {
            var fileExtension = FileFormat == ExportParticipantTemplateFileFormat.Csv ? "csv" : "xlsx";
            return $"Participant-Template.{fileExtension}";
        }
    }
#pragma warning restore CA1024 // Use properties where appropriate
}
