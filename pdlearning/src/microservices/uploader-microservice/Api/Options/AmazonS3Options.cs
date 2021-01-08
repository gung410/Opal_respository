namespace Microservice.Uploader.Options
{
    public class AmazonS3Options
    {
        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string Region { get; set; }

        public string BucketName { get; set; }

        public string TemporaryFolder { get; set; }

        public string PermanentFolder { get; set; }

        public int PartExpirationInSecond { get; set; }

        public int FileExpirationMinutes { get; set; }
    }
}
