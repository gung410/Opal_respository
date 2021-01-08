using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.RequestDtos
{
    public class SaveLearnerUserRequestDtoEntityStatus
    {
        public bool ExternallyMastered { get; set; }

        public LearnerUserStatus Status { get; set; }
    }
}
