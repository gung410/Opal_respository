namespace Microservice.Content.Domain.Entities
{
    public class UploadedContent : DigitalContent
    {
        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileExtension { get; set; }

        public double FileSize { get; set; }

        // This is a combination of {BucketName}/{PermanentFolder}/{FileExtension}/{Id}
        public string FileLocation { get; set; }

        public int FileDuration { get; set; }
    }
}
