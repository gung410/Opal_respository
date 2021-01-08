using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Webinar.Application.Consumers.Messages;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Application.Services;

namespace Microservice.Webinar.Application.Consumers.Meeting
{
    [OpalConsumer("microservice.events.webinar.update-meeting")]
    public class UpdateMeetingConsumer : ScopedOpalMessageConsumer<UpdateMeetingMessage>
    {
        private readonly IWebinarApplicationService _webinarApplicationService;

        public UpdateMeetingConsumer(IWebinarApplicationService webinarApplicationService)
        {
            _webinarApplicationService = webinarApplicationService;
        }

        public async Task InternalHandleAsync(UpdateMeetingMessage message)
        {
            var bookingExists = await _webinarApplicationService.CheckBookingExistsAsync(message.SessionId, message.Source);
            if (bookingExists)
            {
                var request = new UpdateMeetingRequest
                {
                    SessionId = message.SessionId,
                    Title = message.Title,
                    StartTime = message.StartTime,
                    EndTime = message.EndTime,
                    Source = message.Source,
                    PreRecordPath = message.PreRecordPath,
                    PreRecordId = message.PreRecordId,
                    Attendees = message.Attendees.ToList()
                };

                await _webinarApplicationService.UpdateMeeting(request);
            }
            else
            {
                var request = new BookMeetingRequest
                {
                    SessionId = message.SessionId,
                    Title = message.Title,
                    StartTime = message.StartTime,
                    EndTime = message.EndTime,
                    Source = message.Source,
                    Attendees = message.Attendees.ToList()
                };

                await _webinarApplicationService.BookMeeting(request);
            }
        }
    }
}
