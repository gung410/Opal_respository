using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Microservice.WebinarAutoscaler.Configuration;
using Microsoft.Extensions.Options;

namespace Microservice.WebinarAutoscaler.Application.Services.AWSServices
{
    public class EC2InstanceService : IEC2InstanceService
    {
        private readonly AmazonEC2Client _amazonEC2Client;
        private readonly AWSOptions _awsOptions;

        public EC2InstanceService(AmazonEC2Client amazonEC2Client, IOptions<AWSOptions> awsOptions)
        {
            _amazonEC2Client = amazonEC2Client;
            _awsOptions = awsOptions.Value;
        }

        public async Task<List<Instance>> DescribeInstancesAsync(List<string> instanceIds)
        {
            if (instanceIds == null || instanceIds.Count == 0)
            {
                return new List<Instance>();
            }

            DescribeInstancesResponse response = await _amazonEC2Client.DescribeInstancesAsync(new DescribeInstancesRequest
            {
                InstanceIds = instanceIds
            });
            List<Amazon.EC2.Model.Instance> result = response?.Reservations.SelectMany(x => x.Instances).ToList();
            return result;
        }

        public async Task<List<Instance>> DescribeInstancesByContainsNameAsync(string partialName)
        {
            if (string.IsNullOrEmpty(partialName))
            {
                return new List<Instance>();
            }

            DescribeInstancesResponse response = await _amazonEC2Client.DescribeInstancesAsync(new DescribeInstancesRequest
            {
                Filters = new List<Filter>
                {
                    new Filter { Name = "tag:Name", Values = new List<string> { $"{partialName}*" } }
                }
            });
            List<Amazon.EC2.Model.Instance> result = response?.Reservations.SelectMany(x => x.Instances).ToList();
            return result;
        }

        public async Task<Instance> RunInstanceFromLatestVersionLaunchTemplateAsync(string launchTemplateName, string serverName)
        {
            var lastedVersion = await DescribeLaunchTemplateLatestVersionsAsync(launchTemplateName);
            RunInstancesResponse response = await _amazonEC2Client.RunInstancesAsync(new RunInstancesRequest()
            {
                LaunchTemplate = new LaunchTemplateSpecification()
                {
                    LaunchTemplateName = launchTemplateName,
                    Version = lastedVersion
                },
                MinCount = 1,
                MaxCount = 1,
                TagSpecifications = new List<TagSpecification>()
                {
                    new TagSpecification()
                    {
                        Tags = new List<Tag>()
                        {
                            new Tag()
                            {
                                Value = $"{serverName}-v{lastedVersion}",
                                Key = "Name"
                            }
                        },
                        ResourceType = "instance"
                    }
                }
            });

            Instance result = response.Reservation.Instances.FirstOrDefault();
            return result;
        }

        public async Task<bool> TerminateInstanceAsync(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                return true;
            }

            TerminateInstancesResponse response = await _amazonEC2Client.TerminateInstancesAsync(new TerminateInstancesRequest()
            {
                InstanceIds = new List<string>() { instanceId }
            });

            return response.TerminatingInstances.FirstOrDefault() != null;
        }

        public async Task<string> DescribeLaunchTemplateLatestVersionsAsync(string launchTemplateName)
        {
            DescribeLaunchTemplateVersionsResponse response = await _amazonEC2Client.DescribeLaunchTemplateVersionsAsync(new DescribeLaunchTemplateVersionsRequest()
            {
                LaunchTemplateName = launchTemplateName
            });

            var versions = response.LaunchTemplateVersions?.Select(x => x.VersionNumber)?.Max();
            return versions.ToString();
        }
    }
}
