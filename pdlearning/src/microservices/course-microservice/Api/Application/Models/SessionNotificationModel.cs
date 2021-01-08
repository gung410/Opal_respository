using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;

namespace Microservice.Course.Application.Models
{
    public class SessionNotificationModel
    {
        public SessionNotificationModel()
        {
        }

        public SessionNotificationModel(Session entity)
        {
            SessionDate = TimeHelper.ConvertTimeFromUtc(entity.StartDateTime.GetValueOrDefault()).ToString(DateTimeFormatConstant.OnlyDate);
            StartTime = TimeHelper.ConvertTimeFromUtc(entity.StartDateTime.GetValueOrDefault()).ToString(DateTimeFormatConstant.OnlyHourMinute);
            EndTime = TimeHelper.ConvertTimeFromUtc(entity.EndDateTime.GetValueOrDefault()).ToString(DateTimeFormatConstant.OnlyHourMinute);
            Venue = entity.Venue;
        }

        public string SessionDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Venue { get; set; }
    }
}
