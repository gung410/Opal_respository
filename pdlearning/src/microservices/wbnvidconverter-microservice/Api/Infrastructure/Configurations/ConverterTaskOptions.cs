namespace Microservice.WebinarVideoConverter.Infrastructure.Configurations
{
    public class ConverterTaskOptions
    {
        public string TaskDefinition { get; set; }

        public string PlatformVersion { get; set; }

        public string Cluster { get; set; }

        public string SecurityGroupIds { get; set; }

        public string SubnetIds { get; set; }

        public string ConverterContainerName { get; set; }
    }
}
