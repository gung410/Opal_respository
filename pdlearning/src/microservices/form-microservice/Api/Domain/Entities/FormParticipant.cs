using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class FormParticipant : BaseEntity, ISoftDelete
    {
        public Guid UserId { get; set; }

        public Guid FormOriginalObjectId { get; set; }

        public Guid FormId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public bool? IsStarted { get; set; }

        public FormParticipantStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public static Expression<Func<FormParticipant, bool>> IsDoneExpr()
        {
            return x => x.SubmittedDate != null;
        }

        public static Expression<Func<FormParticipant, bool>> HasSaveAnswerPermissionExpr(Guid? userId)
        {
            return x => x.UserId == userId;
        }

        public bool HasSaveAnswerPermission(Guid? userId)
        {
            return HasSaveAnswerPermissionExpr(userId).Compile()(this);
        }
    }
}
