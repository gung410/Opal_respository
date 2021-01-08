using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Webinar.Application.Consumers.Messages;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Application.Services;

namespace Microservice.Webinar.Application.Consumers.Meeting
{
    [OpalConsumer("microservice.events.webinar.book-meeting")]
    public class BookMeetingConsumer : ScopedOpalMessageConsumer<BookMeetingMessage>
    {
        private readonly IWebinarApplicationService _webinarApplicationService;

        public BookMeetingConsumer(IWebinarApplicationService webinarApplicationService)
        {
            _webinarApplicationService = webinarApplicationService;
        }

        public async Task InternalHandleAsync(BookMeetingMessage message)
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
                    Attendees = message.Attendees.ToList(),
                    PreRecordPath = message.PreRecordPath,
                    PreRecordId = message.PreRecordId
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
                    Attendees = message.Attendees.ToList(),
                    PreRecordPath = message.PreRecordPath,
                    PreRecordId = message.PreRecordId
                };

                await _webinarApplicationService.BookMeeting(request);
            }
        }
    }
}
