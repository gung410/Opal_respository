using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Webinar.Application.Consumers.Messages;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Application.Services;

namespace Microservice.Webinar.Application.Consumers.Meeting
{
    [OpalConsumer("microservice.events.webinar.cancel-meeting")]
    public class CancelMeetingConsumer : ScopedOpalMessageConsumer<CancelMeetingMessage>
    {
        private readonly IWebinarApplicationService _webinarApplicationService;

        public CancelMeetingConsumer(IWebinarApplicationService webinarApplicationService)
        {
            _webinarApplicationService = webinarApplicationService;
        }

        public Task InternalHandleAsync(CancelMeetingMessage message)
        {
            var request = new CancelMeetingRequest
            {
                SessionId = message.SessionId,
                Source = message.Source
            };

            return _webinarApplicationService.CancelMeeting(request);
        }
    }
}
