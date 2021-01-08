using System;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from User on the SAM module.
    /// </summary>
    public class LearnerUser : UserEntity, IUserEntity
    {
        public LearnerUserStatus Status { get; set; }

        public LearnerUserAccountType AccountType { get; set; }

        public Guid PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }
    }
}
