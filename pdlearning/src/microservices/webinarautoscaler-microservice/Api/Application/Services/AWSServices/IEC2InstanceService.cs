using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.EC2.Model;

namespace Microservice.WebinarAutoscaler.Application.Services.AWSServices
{
    public interface IEC2InstanceService
    {
        Task<List<Instance>> DescribeInstancesAsync(List<string> instanceIds);

        Task<List<Instance>> DescribeInstancesByContainsNameAsync(string partialName);

        Task<Instance> RunInstanceFromLatestVersionLaunchTemplateAsync(string launchTemplateName, string serverName);

        Task<bool> TerminateInstanceAsync(string instanceId);
    }
}
