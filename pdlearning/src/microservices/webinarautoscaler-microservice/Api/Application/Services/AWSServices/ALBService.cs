using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using Microservice.WebinarAutoscaler.Application.Exception;
using Microservice.WebinarAutoscaler.Configuration;
using Microsoft.Extensions.Options;
using TargetGroup = Amazon.ElasticLoadBalancingV2.Model.TargetGroup;

namespace Microservice.WebinarAutoscaler.Application.Services.AWSServices
{
    public class ALBService : IALBService
    {
        private readonly AmazonElasticLoadBalancingV2Client _amazonElasticLoadBalancingClient;
        private readonly AWSOptions _awsOptions;

        public ALBService(AmazonElasticLoadBalancingV2Client amazonElasticLoadBalancingClient, IOptions<AWSOptions> awsOptions)
        {
            _amazonElasticLoadBalancingClient = amazonElasticLoadBalancingClient;
            _awsOptions = awsOptions.Value;
        }

        public async Task<bool> RegisterTargetAsync(TargetGroup targetGroup, string freeInstanceId)
        {
            var response = await _amazonElasticLoadBalancingClient
                .RegisterTargetsAsync(
                new RegisterTargetsRequest
                {
                    TargetGroupArn = targetGroup.TargetGroupArn,
                    Targets = new List<TargetDescription>
                {
                    new TargetDescription { Id = freeInstanceId }
                }
                });
            return response.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task<TargetGroup> CreateTargetGroupAsync(double newPattern)
        {
            string targetGroupName = $"webinar-bbb-{newPattern}";
            var loadBalancer = await DescribeApplicationLoadBalancerAsync();
            CreateTargetGroupResponse response = await _amazonElasticLoadBalancingClient
                .CreateTargetGroupAsync(
                    new CreateTargetGroupRequest
                    {
                        Name = targetGroupName,
                        Port = 443,
                        Protocol = "HTTPS",
                        VpcId = loadBalancer.VpcId,
                    });

            return response.TargetGroups.FirstOrDefault();
        }

        public async Task<Listener> GetLoadBalancerListener()
        {
            List<Listener> loadBalancerListener = await DescribeListenersAsync();
            Listener httpsListener = loadBalancerListener.SingleOrDefault(x => x.Protocol.Value.Equals("HTTPS"));
            return httpsListener;
        }

        public async Task<List<Listener>> DescribeListenersAsync()
        {
            var loadBalancer = await DescribeApplicationLoadBalancerAsync();
            DescribeListenersResponse response = await _amazonElasticLoadBalancingClient
                .DescribeListenersAsync(
                    new DescribeListenersRequest
                    {
                        LoadBalancerArn = loadBalancer.LoadBalancerArn
                    });
            return response.Listeners;
        }

        public async Task<LoadBalancer> DescribeApplicationLoadBalancerAsync()
        {
            DescribeLoadBalancersResponse response = await _amazonElasticLoadBalancingClient
                .DescribeLoadBalancersAsync(
                    new DescribeLoadBalancersRequest
                    {
                        Names = new List<string> { _awsOptions.LoadBalancerName }
                    });
            return response.LoadBalancers.FirstOrDefault();
        }

        public async Task<int> GetRulePriorityAsync()
        {
            Listener httpsListener = await GetLoadBalancerListener();
            var response = await _amazonElasticLoadBalancingClient
                .DescribeRulesAsync(
                    new DescribeRulesRequest
                    {
                        ListenerArn = httpsListener.ListenerArn
                    });

            for (int i = 1; i < 5000; i++)
            {
                var isExistedPriority = response
                    .Rules
                    .Exists(rule => rule.Priority.Equals(i.ToString()));

                if (!isExistedPriority)
                {
                    return i;
                }
            }

            throw new RulePriorityNoSpacingException();
        }

        public async Task<Rule> CreateRuleAsync(TargetGroup targetGroup, double newPattern)
        {
            Listener httpsListener = await GetLoadBalancerListener();
            int availablePriority = await GetRulePriorityAsync();
            CreateRuleResponse response = await _amazonElasticLoadBalancingClient
                .CreateRuleAsync(
                    new CreateRuleRequest
                    {
                        Actions = new List<Amazon.ElasticLoadBalancingV2.Model.Action>
                        {
                            new Amazon.ElasticLoadBalancingV2.Model.Action
                            {
                                TargetGroupArn = targetGroup.TargetGroupArn,
                                Type = "forward"
                            }
                        },
                        Conditions = new List<RuleCondition>
                        {
                            new RuleCondition
                            {
                                Field = "path-pattern",
                                Values = new List<string>
                                {
                                    $"/bbb-{newPattern}/*"
                                }
                            }
                        },
                        ListenerArn = httpsListener.ListenerArn,
                        Priority = availablePriority
                    });

            return response.Rules.FirstOrDefault();
        }

        public async Task<bool> DeleteTargetGroupAsync(string targetGroupArn)
        {
            if (string.IsNullOrEmpty(targetGroupArn))
            {
                return true;
            }

            DeleteTargetGroupResponse response = await _amazonElasticLoadBalancingClient
                .DeleteTargetGroupAsync(
                    new DeleteTargetGroupRequest
                    {
                        TargetGroupArn = targetGroupArn
                    });

            return response.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> DeleteRuleAsync(string ruleArn)
        {
            if (string.IsNullOrEmpty(ruleArn))
            {
                return true;
            }

            DeleteRuleResponse response = await _amazonElasticLoadBalancingClient
                .DeleteRuleAsync(
                    new DeleteRuleRequest
                    {
                        RuleArn = ruleArn
                    });

            return response.HttpStatusCode == HttpStatusCode.OK;
        }
    }
}
