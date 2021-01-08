using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class ParticipantAssignmentTrack : FullAuditedEntity, ISoftDelete
    {
        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public Guid AssignmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public ParticipantAssignmentTrackStatus Status { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public bool IsAssignedAssessmentManuallyOnce { get; set; }

        public bool IsAutoAssignedOnce { get; set; }

        public virtual ParticipantAssignmentTrackQuizAnswer QuizAnswer { get; set; }

        public bool IsDeleted { get; set; }

        public static Expression<Func<ParticipantAssignmentTrack, bool>> IsDoneExpr()
        {
            return x => x.SubmittedDate != null;
        }

        public static Expression<Func<ParticipantAssignmentTrack, bool>> HasSaveAnswerPermissionExpr(Guid? userId)
        {
            return x => x.UserId == userId;
        }

        public static ExpressionValidator<ParticipantAssignmentTrack> CanSetupPeerAssessmentValidator()
        {
            return new ExpressionValidator<ParticipantAssignmentTrack>(
                p => !p.IsAssignedAssessmentManuallyOnce || !p.IsAutoAssignedOnce,
                "Can not setup peer assessment for assignees who has been assigned assessment manually once or is auto assigned once.");
        }

        public static Validation ValidateCanAssignAssignment(CourseEntity course, ClassRun classRun)
        {
            return Validation.HarvestErrors(
                Validation.ValidIf(
                    classRun.EndDateTime >= Clock.Now,
                    "Cannot assign assignment to assignees due to class run end date is in the past."),
                Validation.ValidIf(
                    !course.IsArchived(),
                    "Cannot assign assignment to assignees due to course is archived."));
        }

        public static bool HasAssignAssignmentPermission(Guid? currentUserId, List<string> currentUserRoles, Func<string, bool> checkHasPermissionFn)
        {
            return currentUserId == null
                   || UserRoles.IsSysAdministrator(currentUserRoles)
                   || checkHasPermissionFn(LearningManagementPermissionKeys.AssignAssignment);
        }

        public bool HasSaveAnswerPermission(Guid? userId)
        {
            return HasSaveAnswerPermissionExpr(userId).Compile()(this);
        }

        public void UpdateSubmittedDate(DateTime submittedDate, int assignmentExtendedDays)
        {
            SubmittedDate = submittedDate;
            if (SubmittedDate <= EndDate)
            {
                Status = ParticipantAssignmentTrackStatus.Completed;
            }
            else if (SubmittedDate > EndDate && SubmittedDate.Value <= EndDate.AddDays(assignmentExtendedDays).EndOfDateInSystemTimeZone().ToUniversalTime())
            {
                Status = ParticipantAssignmentTrackStatus.LateSubmission;
            }
            else
            {
                // Auto submit answer for learner but status will be 'Incomplete'
                Status = ParticipantAssignmentTrackStatus.Incomplete;
            }
        }

        public bool IsDone()
        {
            return IsDoneExpr().Compile()(this);
        }
    }
}
