using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class AttendanceTrackingChangeEvent : BaseThunderEvent
    {
        public AttendanceTrackingChangeEvent(AttendanceTracking attendanceTracking, AttendanceTrackingChangeType changeType)
        {
            AttendanceTracking = attendanceTracking;
            ChangeType = changeType;
        }

        public AttendanceTracking AttendanceTracking { get; }

        public AttendanceTrackingChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.attendancetracking.{ChangeType.ToString().ToLower()}";
        }
    }
}
