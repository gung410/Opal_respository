using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Commands;
using Microservice.WebinarAutoscaler.Application.Queries;
using Microservice.WebinarAutoscaler.Application.RequestDtos;
using Microservice.WebinarAutoscaler.Application.Services.AWSServices;
using Microservice.WebinarAutoscaler.Configuration;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Microsoft.Extensions.Options;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Services
{
    public class BBBServerService : IBBBServerService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IALBService _albService;
        private readonly IEC2InstanceService _ec2InstanceService;
        private readonly AWSOptions _awsOptions;

        public BBBServerService(
            IThunderCqrs thunderCqrs,
            IALBService albService,
            IEC2InstanceService ec2InstanceService,
            IOptions<AWSOptions> awsOptions)
        {
            _thunderCqrs = thunderCqrs;
            _albService = albService;
            _ec2InstanceService = ec2InstanceService;
            _awsOptions = awsOptions.Value;
        }

        public async Task UpdateBBBProtectionStateAsync(UpdateBBBProtectionStateRequest request)
        {
            var isProtectionRequest = request.IsProtection;
            var instanceId = request.InstanceId;
            var getBBBServerByInstanceIdQuery = new GetBBBServerByInstanceIdQuery()
            {
                InstanceId = instanceId
            };
            var bbbServer = await _thunderCqrs.SendQuery(getBBBServerByInstanceIdQuery);
            if (bbbServer == null)
            {
                return;
            }

            // Check IsProtection in DB different with new state
            if (isProtectionRequest != bbbServer.IsProtection)
            {
                var updateBBBServerProtectionStateByIdCommand = new UpdateBBBServerProtectionStateByIdCommand
                {
                    BBBServerId = bbbServer.Id,
                    IsProtection = isProtectionRequest
                };
                await _thunderCqrs.SendCommand(updateBBBServerProtectionStateByIdCommand);
            }
        }

        public async Task<string> GetBBBServerPrivateIpByBBBInstanceIdAsync(string instanceId)
        {
            var getBBBServerByInstanceIdQuery = new GetBBBServerByInstanceIdQuery
            {
                InstanceId = instanceId
            };
            var bbbServer = await _thunderCqrs.SendQuery(getBBBServerByInstanceIdQuery);
            return bbbServer?.PrivateIp;
        }

        public async Task UpdateBBBReadyStatusStateAsync(string instanceId)
        {
            var getBBBServerByInstanceIdQuery = new GetBBBServerByInstanceIdQuery()
            {
                InstanceId = instanceId
            };
            var bbbServer = await _thunderCqrs.SendQuery(getBBBServerByInstanceIdQuery);
            if (bbbServer == null)
            {
                return;
            }

            // Check Status in DB different with new state
            if (bbbServer.Status != BBBServerStatus.Ready)
            {
                // Delay when protection turned off to ensure that server scale out safely
                await Task.Delay(10000);

                var revertUpdateBBBServerProtectionStateByIdCommand = new UpdateBBBServerStatusByIdCommand
                {
                    BBBServerId = bbbServer.Id,
                    Status = BBBServerStatus.Ready
                };
                await _thunderCqrs.SendCommand(revertUpdateBBBServerProtectionStateByIdCommand);
            }
        }

        public async Task UpdateBBBServerMeetingsAsync(UpdateBBBServerMeetingsRequest updateBBBServerMeetingsRequest)
        {
            var bbbServer = await _thunderCqrs.SendQuery(new GetBBBServerByInstanceIdQuery { InstanceId = updateBBBServerMeetingsRequest.InstanceId });
            var meetings = await _thunderCqrs.SendQuery(new GetMeetingsByBBBServerIdQuery { BBBServerId = bbbServer.Id });
            var liveMeetingIds = updateBBBServerMeetingsRequest.MeetingIds;
            foreach (var item in meetings)
            {
                var isLive = item.IsLive;
                if (liveMeetingIds.Contains(item.Id.ToString(), StringComparer.OrdinalIgnoreCase))
                {
                    item.IsLive = true;
                }
                else
                {
                    item.IsLive = false;
                }

                if (isLive != item.IsLive)
                {
                    await _thunderCqrs.SendCommand(new UpdateIsLiveMeetingCommand { Id = item.Id, IsLive = item.IsLive });
                }
            }
        }

        public async Task<List<string>> GetTurnServerPublicIpsAsync()
        {
          // Get all turn servers with running or stop.
          var turnServerIps = (await _ec2InstanceService.DescribeInstancesByContainsNameAsync("video-webinar-turn-app"))
                                .Where(x => !string.IsNullOrEmpty(x.PublicIpAddress))
                                .Select(x => x.PublicIpAddress)
                                .ToList();
          return turnServerIps;
        }
    }
}
