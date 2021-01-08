using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class UserProfile : AuditedEntity, ISoftDelete
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public bool Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfWork { get; set; }

        public TeachingLevel TeachingLevel { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
