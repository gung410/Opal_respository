using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.ECS;
using Amazon.ECS.Model;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core;
using KeyValuePair = Amazon.ECS.Model.KeyValuePair;

namespace Microservice.WebinarVideoConverter.Application.Services
{
    public class AwsFargateTaskRunnerService : IAwsFargateTaskRunnerService
    {
        private readonly ILogger<AwsFargateTaskRunnerService> _logger;
        private readonly AmazonECSClient _amazonECSClient;
        private readonly ConverterTaskOptions _converterTaskOption;

        public AwsFargateTaskRunnerService(
            ILogger<AwsFargateTaskRunnerService> logger,
            AmazonECSClient amazonECSClient,
            IOptions<ConverterTaskOptions> converterTaskOptions)
        {
            _logger = logger;
            _amazonECSClient = amazonECSClient;
            _converterTaskOption = converterTaskOptions.Value;
        }

        public Task<RunTaskResponse> RunConvertPlaybackTaskAsync(string playbackUrl)
        {
            Guard.NotNull(_converterTaskOption.TaskDefinition, nameof(_converterTaskOption.TaskDefinition));
            Guard.NotNull(_converterTaskOption.PlatformVersion, nameof(_converterTaskOption.PlatformVersion));
            Guard.NotNull(_converterTaskOption.SecurityGroupIds, nameof(_converterTaskOption.SecurityGroupIds));
            Guard.NotNull(_converterTaskOption.SubnetIds, nameof(_converterTaskOption.SubnetIds));

            var securityGroups = _converterTaskOption.SecurityGroupIds.Split(',').ToList();
            var subnetIds = _converterTaskOption.SubnetIds.Split(',').ToList();

            Guard.NotEmpty(securityGroups, nameof(_converterTaskOption.SecurityGroupIds));
            Guard.NotEmpty(subnetIds, nameof(_converterTaskOption.SubnetIds));

            return _amazonECSClient.RunTaskAsync(new RunTaskRequest
            {
                LaunchType = LaunchType.FARGATE,
                TaskDefinition = _converterTaskOption.TaskDefinition,
                PlatformVersion = _converterTaskOption.PlatformVersion,
                Cluster = _converterTaskOption.Cluster,
                Count = 1,
                NetworkConfiguration = new NetworkConfiguration
                {
                    AwsvpcConfiguration = new AwsVpcConfiguration
                    {
                        AssignPublicIp = AssignPublicIp.DISABLED,
                        SecurityGroups = securityGroups,
                        Subnets = subnetIds
                    }
                },
                EnableECSManagedTags = true,
                Overrides = new TaskOverride
                {
                    ContainerOverrides = new List<ContainerOverride>
                    {
                        new ContainerOverride
                        {
                            Name = _converterTaskOption.ConverterContainerName,
                            Environment = new List<KeyValuePair>
                            {
                                new KeyValuePair
                                {
                                    Name = "MEETING_URL",
                                    Value = playbackUrl
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
