using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class SurveyParticipant : BaseEntity, ISoftDelete
    {
        public Guid UserId { get; set; }

        public Guid SurveyOriginalObjectId { get; set; }

        public Guid SurveyId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public bool? IsStarted { get; set; }

        public SurveyParticipantStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public static Expression<Func<SurveyParticipant, bool>> IsDoneExpr()
        {
            return x => x.SubmittedDate != null;
        }

        public static Expression<Func<SurveyParticipant, bool>> HasSaveAnswerPermissionExpr(Guid? userId)
        {
            return x => x.UserId == userId;
        }

        public bool HasSaveAnswerPermission(Guid? userId)
        {
            return HasSaveAnswerPermissionExpr(userId).Compile()(this);
        }
    }
}
