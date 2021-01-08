using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Webinar.Application.Commands;
using Microservice.Webinar.Application.Consumers.Messages;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Consumers.Meeting
{
    [OpalConsumer("microservice.events.webinarautoscaler.bbb-server-private-ip-updated")]
    public class UpdateMeetingPrivateIpComsumer : ScopedOpalMessageConsumer<UpdateMeetingPrivateIpMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ILogger<UpdateMeetingPrivateIpComsumer> _logger;

        public UpdateMeetingPrivateIpComsumer(
            IThunderCqrs thunderCqrs,
            ILogger<UpdateMeetingPrivateIpComsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _logger = logger;
        }

        public async Task InternalHandleAsync(UpdateMeetingPrivateIpMessage message)
        {
            _logger.LogInformation(
                "{NameOfConsumer}: Message received, meetingId: {MeetingId}, privateIp: {PrivateIp}",
                nameof(UpdateMeetingPrivateIpComsumer),
                message.MeetingId,
                message.BBBServerPrivateIp);

            var command = new UpdateMeetingPrivateIpCommand
            {
                MeetingId = message.MeetingId,
                BBBServerPrivateIp = message.BBBServerPrivateIp
            };

            await _thunderCqrs.SendCommand(command);
        }
    }
}
