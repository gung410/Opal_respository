using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Domain.Entities
{
    public class QuizAssignmentForm : FullAuditedEntity, ISoftDelete
    {
        [JsonIgnore]
        public virtual ICollection<ParticipantAssignmentTrackQuizAnswer> Answers { get; set; }

        public bool IsDeleted { get; set; }

        public bool RandomizedQuestions { get; set; }

        public QuizAssignmentForm Clone(Guid newAssignmentId)
        {
            return new QuizAssignmentForm
            {
                Id = newAssignmentId,
                CreatedDate = Clock.Now,
                RandomizedQuestions = RandomizedQuestions
            };
        }
    }
}
