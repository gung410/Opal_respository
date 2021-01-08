namespace Microservice.WebinarVideoConverter.Infrastructure.Configurations
{
    public class AmazonS3Options
    {
        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string BucketName { get; set; }

        public string Region { get; set; }

        public string WebinarPlaybacksDir { get; set; }
    }
}
