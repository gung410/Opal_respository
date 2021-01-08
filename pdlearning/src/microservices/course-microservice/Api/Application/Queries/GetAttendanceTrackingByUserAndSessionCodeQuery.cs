using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAttendanceTrackingByUserAndSessionCodeQuery : BaseThunderQuery<AttendanceTrackingModel>
    {
        public string SessionCode { get; set; }
    }
}
