using System;

namespace Microservice.Learner.Application.RequestDtos
{
    public class SaveLearnerUserRequestDtoIdentity
    {
        public int Id { get; set; }

        public Guid? ExtId { get; set; }
    }
}
