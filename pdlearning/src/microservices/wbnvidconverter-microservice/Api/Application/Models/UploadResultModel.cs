namespace Microservice.WebinarVideoConverter.Application.Models
{
    public class UploadResultModel
    {
        public bool IsSuccess { get; set; } = false;

        public string FilePath { get; set; }

        public double FileSize { get; set; }
    }
}
