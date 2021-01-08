using Amazon.ECS;
using Microservice.WebinarVideoConverter.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.WebinarVideoConverter.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwsClient(this IServiceCollection services, IConfiguration configuration)
        {
            var awsOptions = new AWSOptions();
            configuration.Bind(nameof(AWSOptions), awsOptions);

            services.AddTransient<AmazonECSClient>(sp =>
                new AmazonECSClient(
                    awsOptions.AccessKey,
                    awsOptions.SecretKey,
                    awsOptions.RegionEndpoint));

            return services;
        }
    }
}
