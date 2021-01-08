using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class AttendanceTracking : FullAuditedEntity, ISoftDelete
    {
        public Guid SessionId { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid Userid { get; set; }

        public string ReasonForAbsence { get; set; }

        public IEnumerable<string> Attachment { get; set; }

        public AttendanceTrackingStatus? Status { get; set; } = null;

        public bool IsCodeScanned { get; set; } = false;

        public DateTime? CodeScannedDate { get; set; }

        public bool IsDeleted { get; set; }

        public static Expression<Func<AttendanceTracking, bool>> HasOwnerPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null || UserRoles.IsSysAdministrator(userRoles) || x.Userid == userId;
        }

        public static Expression<Func<AttendanceTracking, bool>> MissingAttendanceInfoExpr()
        {
            return p => p.IsCodeScanned == false && p.Status == null;
        }

        public static Expression<Func<AttendanceTracking, bool>> IsAttendanceCheckingCompletedExpr()
        {
            return p => (!p.Status.HasValue && p.IsCodeScanned) || p.Status == AttendanceTrackingStatus.Present;
        }

        public static bool HasSetPresentAbsentPermission(CourseEntity course, Guid? userId, IEnumerable<string> userRoles, Func<string, bool> checkHasPermissionFn)
        {
            return course.HasViewContentPermission(userId, userRoles) && checkHasPermissionFn(LearningManagementPermissionKeys.SetPresentAbsent);
        }

        public static Validation ValidateCanSetPresentAbsent(CourseEntity course, ClassRun classRun)
        {
            return Validation.ValidIf(
                course.IsNotArchived() && classRun.StartDateTime != null,
                "ClassRun StartDateTime must be valid and course is not archived");
        }

        public Validation ValidateCanScanCodeForCheckIn(CourseEntity course, Session session)
        {
            return Validation.FailFast(
                () => course.ValidateNotArchived(),
                () => Validation.ValidIf(session.Started(), "Unable to check-in now, the code is only available when the session starts."));
        }

        public bool IsAttendanceCheckingCompleted()
        {
            return IsAttendanceCheckingCompletedExpr().Compile()(this);
        }

        public bool HasOwnerPermission(Guid? userId, IEnumerable<string> userRoles)
        {
            return HasOwnerPermissionExpr(userId, userRoles).Compile()(this);
        }
    }
}
