using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    public class MyOutstandingTask : AuditedEntity
    {
        /// <summary>
        /// The identifier user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// <para>ItemId is stored as the identifier registration if the task type is <see cref="OutstandingTaskType.Course"/>.</para>
        /// <para>ItemId is stored as the identifier <see cref="MyAssignment"/> if the task type is <see cref="OutstandingTaskType.Assignment"/>.</para>
        /// <para>ItemId is stored as the identifier <see cref="MyCourse"/> if the task type is <see cref="OutstandingTaskType.Microlearning"/>.</para>
        /// <para>ItemId is stored as the identifier <see cref="MyDigitalContent"/> if the task type is <see cref="OutstandingTaskType.DigitalContent"/>.</para>
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// Type of task.
        /// </summary>
        public OutstandingTaskType ItemType { get; set; }

        public int Priority { get; set; }

        public DateTime? DueDate { get; set; }

        public void WithDueDate(DateTime? dueDate)
        {
            if (dueDate != null)
            {
                DueDate = dueDate;
            }
        }
    }
}
