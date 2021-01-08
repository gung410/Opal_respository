using System;

namespace Microservice.Learner.Application.RequestDtos
{
    public class SaveLearnerUserRequestDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public int DepartmentId { get; set; }

        public SaveLearnerUserRequestDtoIdentity Identity { get; set; }

        public SaveLearnerUserRequestDtoEntityStatus EntityStatus { get; set; }

        public Guid PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }
    }
}
