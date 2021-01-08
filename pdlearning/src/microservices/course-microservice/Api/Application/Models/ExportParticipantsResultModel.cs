using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.Models
{
    public class ExportParticipantsResultModel
    {
        public CourseModel Course { get; set; }

        public byte[] FileContent { get; set; }

        public ExportParticipantsFileFormat FileFormat { get; set; }

#pragma warning disable CA1024 // Use properties where appropriate
        public string GetFileName()
#pragma warning restore CA1024 // Use properties where appropriate
        {
            var fileExtension = FileFormat == ExportParticipantsFileFormat.Csv ? "csv" : "xlsx";
            return $"Participants of [{Course.CourseName}] course.{fileExtension}";
        }
    }
}
