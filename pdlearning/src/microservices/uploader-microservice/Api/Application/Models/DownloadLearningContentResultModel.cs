using Microservice.Uploader.Application.Commands.Enums;

namespace Microservice.Uploader.Application.Models
{
    public class DownloadLearningContentResultModel
    {
        public byte[] HtmlContentConverted { get; set; }

        public FileFormat FileFormat { get; set; }
    }
}
