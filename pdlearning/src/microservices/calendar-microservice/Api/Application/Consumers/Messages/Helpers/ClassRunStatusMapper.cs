using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages.Helpers
{
    public static class ClassRunStatusMapper
    {
        /// <summary>
        /// Get classrun event status base on Course and ClassRun status.
        /// Event has Opening status when Course and ClassRun is Publish.
        /// </summary>
        /// <param name="classRunStatus">ClassRun status.</param>
        /// <param name="courseStatus">Course status.</param>
        /// <returns>Event status.</returns>
        public static EventStatus GetClassRunEventStatus(ClassRunStatus classRunStatus, CourseStatus courseStatus)
        {
            return courseStatus == CourseStatus.Published && classRunStatus == ClassRunStatus.Published
                ? EventStatus.Opening
                : EventStatus.Building;
        }
    }
}
