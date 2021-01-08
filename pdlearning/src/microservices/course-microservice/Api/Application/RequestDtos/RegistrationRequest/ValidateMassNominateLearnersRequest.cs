using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class ValidateMassNominateLearnersRequest
    {
        public List<ValidateMassNominateLearnerDto> ValidateNominatedLearners { get; set; }
    }
}
