using Amazon.AutoScaling;
using Amazon.EC2;
using Amazon.ElasticLoadBalancingV2;
using Microservice.WebinarAutoscaler.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.WebinarAutoscaler.Common.Extensions
{
    public static class ServiceExtension
    {
        public static void AddAwsClient(this IServiceCollection services, IConfiguration configuration)
        {
            var awsOptions = new AWSOptions();
            configuration.Bind(nameof(AWSOptions), awsOptions);

            services.AddTransient<AmazonAutoScalingClient>(sp =>
                new AmazonAutoScalingClient(
                    awsOptions.AccessKey,
                    awsOptions.SecretKey,
                    awsOptions.RegionEndpoint));
            services.AddTransient<AmazonEC2Client>(sp =>
                new AmazonEC2Client(
                    awsOptions.AccessKey,
                    awsOptions.SecretKey,
                    awsOptions.RegionEndpoint));
            services.AddTransient<AmazonElasticLoadBalancingV2Client>(sp =>
                new AmazonElasticLoadBalancingV2Client(
                    awsOptions.AccessKey,
                    awsOptions.SecretKey,
                    awsOptions.RegionEndpoint));
        }
    }
}
