using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class DigitalContentConfigModel
    {
        public bool CanDownload { get; set; }

        public LectureDigitalContentConfig ToLectureDigitalContentConfig()
        {
            return new LectureDigitalContentConfig
            {
                CanDownload = CanDownload
            };
        }
    }
}
