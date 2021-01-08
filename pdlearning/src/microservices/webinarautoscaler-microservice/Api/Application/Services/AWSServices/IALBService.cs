using System.Threading.Tasks;
using Amazon.ElasticLoadBalancingV2.Model;

namespace Microservice.WebinarAutoscaler.Application.Services.AWSServices
{
    public interface IALBService
    {
        Task<TargetGroup> CreateTargetGroupAsync(double newPattern);

        Task<Rule> CreateRuleAsync(TargetGroup targetGroup, double newPattern);

        Task<bool> RegisterTargetAsync(TargetGroup targetGroup, string freeInstanceId);

        Task<bool> DeleteTargetGroupAsync(string targetGroupArn);

        Task<bool> DeleteRuleAsync(string ruleArn);
    }
}
