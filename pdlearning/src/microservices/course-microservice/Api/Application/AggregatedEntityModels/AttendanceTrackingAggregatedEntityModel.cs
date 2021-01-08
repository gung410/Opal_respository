using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class AttendanceTrackingAggregatedEntityModel : BaseAggregatedEntityModel
    {
        public AttendanceTracking AttendanceTracking { get; private set; }

        public CourseEntity Course { get; private set; }

        public ClassRun ClassRun { get; private set; }

        public Registration Registration { get; private set; }

        public static AttendanceTrackingAggregatedEntityModel Create(
            AttendanceTracking attendanceTracking,
            CourseEntity course = null,
            ClassRun classRun = null,
            Registration registration = null)
        {
            return new AttendanceTrackingAggregatedEntityModel
            {
                AttendanceTracking = attendanceTracking,
                Course = course,
                ClassRun = classRun,
                Registration = registration
            };
        }

        public static AttendanceTrackingAggregatedEntityModel New(AttendanceTracking attendanceTracking)
        {
            return Create(attendanceTracking);
        }

        public AttendanceTrackingAggregatedEntityModel WithRegistration(Registration registration)
        {
            Registration = registration;

            return this;
        }

        public AttendanceTrackingAggregatedEntityModel WithCourse(CourseEntity course)
        {
            Course = course;

            return this;
        }

        public AttendanceTrackingAggregatedEntityModel WithClassRun(ClassRun classRun)
        {
            ClassRun = classRun;

            return this;
        }
    }
}
