using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using Microservice.Webinar.Application.Events;
using Microservice.WebinarAutoscaler.Application.Commands;
using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Application.Queries;
using Microservice.WebinarAutoscaler.Application.RequestDtos;
using Microservice.WebinarAutoscaler.Application.Services.AWSServices;
using Microservice.WebinarAutoscaler.Common.Extensions;
using Microservice.WebinarAutoscaler.Configuration;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Services
{
    public class BBBSyncService : IBBBSyncService
    {
        private readonly AWSOptions _awsOptions;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IEC2InstanceService _ec2InstanceService;
        private readonly IALBService _albService;
        private readonly ILogger<BBBSyncService> _logger;

        public BBBSyncService(
            IThunderCqrs thunderCqrs,
            IEC2InstanceService ec2InstanceService,
            IALBService albService,
            ILogger<BBBSyncService> logger,
            IOptions<AWSOptions> awsOptions)
        {
            _awsOptions = awsOptions.Value;
            _thunderCqrs = thunderCqrs;
            _ec2InstanceService = ec2InstanceService;
            _albService = albService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task ScaleOutBBBServerAsync()
        {
            // Count BBB Servers not belong to any meetings.
            var availableBBBServerCount = await _thunderCqrs.SendQuery(new CountBBBServersForScalingOutQuery());
            if (availableBBBServerCount == 0)
            {
                _logger.LogInformation("Detect: {availableBBBServerCount} server available, Action: scale out 1 new server.", availableBBBServerCount);

                // Create new BBB server for meetings
                var bbbInstance = await _ec2InstanceService.RunInstanceFromLatestVersionLaunchTemplateAsync(_awsOptions.BBBLaunchTemplateName, "main-webinar-bbb");
                await RegisterToAWSAsync(bbbInstance);
            }
        }

        /// <inheritdoc />
        public async Task ScaleInBBBServerAsync()
        {
            // Get all bbbServers that have all owned meetings ended or canceled
            var bbbServerIds = await _thunderCqrs.SendQuery(new GetBBBServerIdsForScalingInQuery());

            foreach (var bbbServerId in bbbServerIds)
            {
                _logger.LogInformation("Detect: server id - {bbbServerId} no meeting, Action: scale in 1 server.", bbbServerId);
                await UnRegisterToAWSAsync(bbbServerId);
            }
        }

        /// <inheritdoc />
        public async Task SeekCoordinatedBBBServerAsync()
        {
            // Get all meetings have no bbbServerId
            var meetingsWithNoBBBServer = await _thunderCqrs.SendQuery(new GetMeetingsWithoutBBBServerInMeetingTime());
            var maxParticipantInOneServer = _awsOptions.MaxParticipantInOneServer;
            foreach (var meeting in meetingsWithNoBBBServer)
            {
                // Get 1 BBB Server ready for meeting(not cancel, )
                var bbbServers = await _thunderCqrs.SendQuery(new GetBBBServersQuery());
                foreach (var server in bbbServers)
                {
                    // To guarantee that number of participants under setting default value.
                    _logger.LogInformation(
                        "ServerParticipantCount: {ServerParticipantCount}, " +
                        "MeetingParticipantCount: {MeetingParticipantCount}, " +
                        "MaxCapacity: {MaxParticipantInOneServer}.",
                        server.ParticipantCount,
                        meeting.ParticipantCount,
                        maxParticipantInOneServer);

                    if (server.ParticipantCount + meeting.ParticipantCount <= maxParticipantInOneServer)
                    {
                        // Update meeting BBBServerId
                        await AssignBBBServerToMeeting(server, meeting);
                        break;
                    }

                    // if the meeting has a higher participants than max size, that must be an empty server.
                    else if (server.ParticipantCount == 0 && meeting.ParticipantCount > maxParticipantInOneServer)
                    {
                        await AssignBBBServerToMeeting(server, meeting);
                        break;
                    }
                    else
                    {
                        _logger.LogInformation("server id: {ServerId} is not enough capacity for meeting id: {MeetingId}.", server.Id, meeting.Id);
                    }
                }
            }
        }

        private async Task AssignBBBServerToMeeting(BBBServerModel server, MeetingInfoModel meeting)
        {
            _logger.LogInformation("Assign server  - {ServerId} no meeting, Action: assign to meeting id: {MeetingId}.", server.Id, meeting.Id);
            await _thunderCqrs.SendCommand(
                       new UpdateMeetingInfoBBBServerIdCommand
                       {
                           Id = meeting.Id,
                           BBBServerId = server.Id
                       });
            await SentEventUpdateBBBServerPrivateIp(meeting.Id, server?.PrivateIp);
        }

        private async Task SentEventUpdateBBBServerPrivateIp(Guid meetingId, string bbbServerPrivateIp)
        {
            await _thunderCqrs.SendEvent(
                new MeetingChangeEvent(
                    new MeetingInfoRequest
                    {
                        MeetingId = meetingId,
                        BBBServerPrivateIp = bbbServerPrivateIp
                    },
                    MeetingChangeType.Updated));
        }

        /// <summary>
        /// Unregister BBB instance to ALB, Target group, Rule.
        /// </summary>
        /// <param name="bbbServerId">bbb server information.</param>
        /// <returns>representing the asynchronous operation.</returns>
        private async Task UnRegisterToAWSAsync(Guid bbbServerId)
        {
            var bbbServer = await _thunderCqrs.SendQuery(new GetBBBServerByIdQuery
            {
                BBBServerId = bbbServerId
            });
            await _thunderCqrs.SendCommand(new DeleteBBBServerCommand
            {
                BBBServerId = bbbServerId
            });
            await _albService.DeleteRuleAsync(bbbServer.RuleArn);
            await _albService.DeleteTargetGroupAsync(bbbServer.TargetGroupArn);
            await _ec2InstanceService.TerminateInstanceAsync(bbbServer.InstanceId);
        }

        /// <summary>
        /// Save BBB server to DB, update AWS resources (target group, alb rule)
        /// If failed to update, undo changed from db.
        /// </summary>F
        /// <param name="bbbInstance">bbb Server information.</param>
        /// <returns>Task.Completed.</returns>
        private async Task RegisterToAWSAsync(Instance bbbInstance)
        {
            try
            {
                var saveBBBServerCommand = new SaveBBBServerCommand
                {
                    InstanceId = bbbInstance.InstanceId,
                    PrivateIp = bbbInstance.PrivateIpAddress,
                    Status = BBBServerStatus.New
                };

                // This needs to throw exception if the bbb existed in db.
                await _thunderCqrs.SendCommand(saveBBBServerCommand);

                var getBBBServerByInstanceIdQuery = new GetBBBServerByInstanceIdQuery()
                {
                    InstanceId = bbbInstance.InstanceId
                };
                var registeredBBBServer = await _thunderCqrs.SendQuery(getBBBServerByInstanceIdQuery);

                var patternChain = bbbInstance.PrivateIpAddress.ConvertIpAddressToPattern();
                var newPattern = double.Parse(patternChain);

                // Create new target group.
                var targetGroup = await _albService.CreateTargetGroupAsync(newPattern);
                if (targetGroup == null)
                {
                    throw new BusinessLogicException("Could not create new target group.");
                }

                registeredBBBServer.TargetGroupArn = targetGroup.TargetGroupArn;

                // Place BBB instance to the target group.
                var isBBBRunning = await IsWaitUntilBBBServerRunning(bbbInstance.InstanceId);
                if (!isBBBRunning)
                {
                    throw new BusinessLogicException("BBB is not running.");
                }

                var isTargetRegister = await _albService.RegisterTargetAsync(targetGroup, bbbInstance.InstanceId);
                if (!isTargetRegister)
                {
                    throw new BusinessLogicException("Could not place BBB instance to the target group.");
                }

                // Add rule to alb rule.
                var rule = await _albService.CreateRuleAsync(targetGroup, newPattern);
                if (rule == null)
                {
                    throw new BusinessLogicException("Could not create rule to alb.");
                }

                registeredBBBServer.RuleArn = rule.RuleArn;

                var updateBBBServerCommand = new UpdateBBBServerCommand
                {
                    BBBServer = registeredBBBServer
                };
                await _thunderCqrs.SendCommand(updateBBBServerCommand);
            }
            catch (System.Exception ex)
            {
                // remove/undo BBB server if failed to update AWS states.
                var getBBBServerByInstanceIdQuery = new GetBBBServerByInstanceIdQuery()
                {
                    InstanceId = bbbInstance.InstanceId
                };
                var registeredBBBServer = await _thunderCqrs.SendQuery(getBBBServerByInstanceIdQuery);
                var command = new DeleteBBBServerCommand
                {
                    BBBServerId = registeredBBBServer.Id
                };
                await _thunderCqrs.SendCommand(command);
                _logger.LogError("Failed to register to alb: {Exception}.", ex.ToString());
            }
        }

        private async Task<bool> IsWaitUntilBBBServerRunning(string instanceId)
        {
            int y = 20000;
            while (y > 0)
            {
                await Task.Delay(100);
                var bbb = (await _ec2InstanceService.DescribeInstancesAsync(
                        new List<string>
                        {
                               instanceId
                        }))
                    .FirstOrDefault();

                // Check whether public ip assigned.
                if (bbb != null && bbb.State.Name.Value.Equals("running"))
                {
                    return true;
                }
                else if (bbb != null && bbb.State.Name.Value.Equals("stopped"))
                {
                    return false;
                }

                y--;
            }

            return false;
        }
    }
}
