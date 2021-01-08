using Amazon;

namespace Microservice.WebinarVideoConverter.Configuration
{
    public class AWSOptions
    {
        private RegionEndpoint _regionEndpoint;

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string Region { get; set; }

        public RegionEndpoint RegionEndpoint
        {
            get
            {
                return _regionEndpoint ??= RegionEndpoint.GetBySystemName(Region);
            }
        }
    }
}
