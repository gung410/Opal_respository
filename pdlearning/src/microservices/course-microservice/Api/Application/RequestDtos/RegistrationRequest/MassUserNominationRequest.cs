using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class MassUserNominationRequest
    {
        public List<MassNominatedRegistrationDto> NominatedLearners { get; set; }
    }
}
