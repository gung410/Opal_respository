using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microservice.Course.Common.Extensions;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Domain.Entities
{
    /// <summary>
    /// This entity present an assessment answer for a participant assignment.
    /// </summary>
    public class AssessmentAnswer : FullAuditedEntity, ISoftDelete
    {
        public Guid AssessmentId { get; set; }

        public Guid ParticipantAssignmentTrackId { get; set; }

        /// <summary>
        /// The user who do this assessment answer.
        /// </summary>
        public Guid UserId { get; set; }

        public IEnumerable<AssessmentCriteriaAnswer> CriteriaAnswers { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public bool IsDeleted { get; set; }

        public static Expression<Func<AssessmentAnswer, bool>> IsDoneExpr()
        {
            return x => x.SubmittedDate != null;
        }

        public static Expression<Func<AssessmentAnswer, bool>> IsPeerAssessmentExpr()
        {
            return x => x.UserId != Guid.Empty;
        }

        public static Expression<Func<AssessmentAnswer, bool>> IsAssignedPeerAssessmentExpr(Guid participantAssignmentTrackId, Guid userId)
        {
            return x => x.ParticipantAssignmentTrackId == participantAssignmentTrackId && x.UserId == userId;
        }

        public static Expression<Func<AssessmentAnswer, bool>> IsSubmittedAssessmentAnswerExpr()
        {
            return x => x.SubmittedDate.HasValue;
        }

        public static Expression<Func<AssessmentAnswer, bool>> IsNotSubmittedAssessmentAnswerExpr()
        {
            return IsSubmittedAssessmentAnswerExpr().Not();
        }

        public static ExpressionValidator<AssessmentAnswer> CanDeleteValidator()
        {
            return new ExpressionValidator<AssessmentAnswer>(
                IsNotSubmittedAssessmentAnswerExpr(),
                "Cannot delete submitted assessment.");
        }

        public bool IsPeerAssessment()
        {
            return IsPeerAssessmentExpr().Compile()(this);
        }

        public bool IsDone()
        {
            return IsDoneExpr().Compile()(this);
        }

        public Validation ValidateCanDelete()
        {
            return CanDeleteValidator().Validate(this);
        }
    }

    public class AssessmentCriteriaAnswer
    {
        public Guid CriteriaId { get; set; }

        public Guid ScaleId { get; set; }

        public string Comment { get; set; }
    }
}
