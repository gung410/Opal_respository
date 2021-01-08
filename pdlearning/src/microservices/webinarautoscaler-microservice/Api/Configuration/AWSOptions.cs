using Amazon;

namespace Microservice.WebinarAutoscaler.Configuration
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
                return _regionEndpoint ??= Amazon.RegionEndpoint.GetBySystemName(Region);
            }
        }

        public string AutoScaleGroupName { get; set; }

        public string LoadBalancerName { get; set; }

        public string TargetGroupVPC { get; set; }

        public string WebinarDns { get; set; }

        public string BBBLaunchTemplateName { get; set; }

        public int MaxParticipantInOneServer { get; set; }
    }
}
