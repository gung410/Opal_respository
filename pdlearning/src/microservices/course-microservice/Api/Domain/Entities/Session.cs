using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Course.Application.Constants;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class Session : FullAuditedEntity, ISoftDelete
    {
        public static readonly int SessionCodeLength = 3;
        public static readonly int MinDaysBeforeStartToUpdateMeeting = 2;

        public Guid ClassRunId { get; set; }

        public string SessionTitle { get; set; }

        public string Venue { get; set; }

        /// <summary>
        /// Online if true, offline if false.
        /// </summary>
        public bool LearningMethod { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public string ExternalId { get; set; }

        public string SessionCode { get; set; }

        public bool UsePreRecordClip { get; set; }

        /// <summary>
        /// This is video path of PreRecordId from ccpm. This is a relative S3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        /// <summary>
        /// This is video content id from ccpm.
        /// </summary>
        public Guid? PreRecordId { get; set; }

        public bool IsDeleted { get; set; }

        public static Validation ValidateCanCreateSession(ClassRun classRun, CourseEntity course)
        {
            return Validation.ValidIf(
                classRun.Status != ClassRunStatus.Cancelled && !classRun.Started() && course.IsNotArchived(),
                "Cannot create session for cancelled class run or started classrun.");
        }

        public static Expression<Func<Session, bool>> CanEditOrDeleteSessionExpr(ClassRun classRun, CourseEntity course)
        {
            return course.IsArchived() || classRun.Status != ClassRunStatus.Unpublished || classRun.Started()
                ? (Expression<Func<Session, bool>>)(session => false)
                : session => true;
        }

        public static Expression<Func<Session, bool>> CanEditOrDeletePreRecordInSessionExpr(ClassRun classRun, CourseEntity course)
        {
            if (course.IsArchived() || classRun.Status != ClassRunStatus.Unpublished)
            {
                return p => false;
            }

            if (!classRun.Started())
            {
                return p => true;
            }

            return StartedExpr().Not();
        }

        public static Expression<Func<Session, bool>> StartedExpr()
        {
            return p => p.StartDateTime <= Clock.Now;
        }

        public static ExpressionValidator<Session> CanChangeLearningMethodValidator(CourseEntity course)
        {
            // Can change learning method before sessionDate at least 1 day.
            var next24HoursFromNow = Clock.Now.AddDays(1);

            return new ExpressionValidator<Session>(
                session => course.IsNotArchived() && (session.StartDateTime == null || next24HoursFromNow < session.StartDateTime),
                "Cannot change learning method type due to this action must be done at least 1 day before the session start day.");
        }

        public static Expression<Func<Session, bool>> IsLearningOnlineExpr()
        {
            return session => session.LearningMethod;
        }

        public static Expression<Func<Session, bool>> CanBookMeetingExpr(int maxMinutesCanBookEarly)
        {
            return IsLearningOnlineExpr()
                .AndAlso(session => session.StartDateTime.HasValue && session.EndDateTime.HasValue &&
                                    session.StartDateTime >= Clock.Now.AddMinutes(maxMinutesCanBookEarly) &&
                                    session.StartDateTime < Clock.Now.AddMinutes(maxMinutesCanBookEarly).AddDays(MinDaysBeforeStartToUpdateMeeting));
        }

        public static Expression<Func<Session, bool>> CanUpdateMeetingExpr()
        {
            return IsLearningOnlineExpr()
                .AndAlso(session => session.StartDateTime.HasValue && session.EndDateTime.HasValue &&
                                    session.StartDateTime < Clock.Now.AddDays(MinDaysBeforeStartToUpdateMeeting) &&
                                    session.EndDateTime > Clock.Now);
        }

        public static bool HasCreateEditPermission(Guid? currentUserId, List<string> currentUserRoles, Func<string, bool> checkHasPermissionFn)
        {
            return currentUserId == null
                   || UserRoles.IsSysAdministrator(currentUserRoles)
                   || checkHasPermissionFn(CourseAdminManagementPermissionKeys.CreateEditSession);
        }

        public static bool HasGetSessionCodePermission(Guid? currentUserId, List<string> currentUserRoles, Func<string, bool> checkHasPermissionFn)
        {
            return currentUserId == null
                   || UserRoles.IsSysAdministrator(currentUserRoles)
                   || checkHasPermissionFn(LearningManagementPermissionKeys.GetSessionCode);
        }

        public bool HasModifyPermission(Guid? userId, CourseEntity course, bool haveFullRight)
        {
            return userId == null
              || haveFullRight
              || course.FirstAdministratorId == userId
              || course.SecondAdministratorId == userId;
        }

        public Validation ValidateCanEditOrDelete(ClassRun classRun, CourseEntity course)
        {
            return Validation.ValidIf(
                CanEditOrDeleteSessionExpr(classRun, course).Compile()(this),
                "Cannot edit/delete session for started or not unpublished class run.");
        }

        public Validation CanEditOrDeletePreRecordInSession(ClassRun classRun, CourseEntity course)
        {
            return Validation.ValidIf(
                CanEditOrDeletePreRecordInSessionExpr(classRun, course).Compile()(this),
                "Cannot edit/delete pre-record clip for started session or not unpublished class run.");
        }

        public bool CanBookMeeting(int validMinutes = 0)
        {
            return CanBookMeetingExpr(validMinutes).Compile()(this);
        }

        public Validation ValidateCanChangeLearningMethod(CourseEntity course)
        {
            return CanChangeLearningMethodValidator(course).Validate(this);
        }

        public bool IsLearningOnline()
        {
            return LearningMethod;
        }

        public bool Started()
        {
            return StartedExpr().Compile()(this);
        }
    }
}
