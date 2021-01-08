using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Domain.Entities
{
    public class Announcement : FullAuditedEntity, ISoftDelete
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime? SentDate { get; set; }

        public DateTime? ScheduleDate { get; set; }

        /// <summary>
        /// Participant registration ids.
        /// </summary>
        public IEnumerable<Guid> Participants { get; set; } = new List<Guid>();

        public AnnouncementStatus Status { get; set; } = AnnouncementStatus.Scheduled;

        public Guid CourseId { get; set; }

        public Guid ClassrunId { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public static ExpressionValidator<Announcement> CanModifyValidator(CourseEntity course)
        {
            return new ExpressionValidator<Announcement>(
                p => course.IsNotArchived() && p.Status == AnnouncementStatus.Scheduled,
                "Announcement has been Sent/Cancelled");
        }

        public static bool HasCudPermission(Guid? userId, List<string> userRoles, CourseEntity course, ClassRun classRun, Func<CourseEntity, bool> haveCourseFullRight)
        {
            return Registration.CanManageRegistrations(userId, course, classRun, userRoles, haveCourseFullRight);
        }

        public Validation ValidateCanModify(CourseEntity course)
        {
            return CanModifyValidator(course).Validate(this);
        }

        public Validation ValidateSendAnnouncementNotifyLearner()
        {
            return Validation.ValidIf(Status == AnnouncementStatus.Sent, "Announcement status must be Sent to notify learner");
        }
    }
}
