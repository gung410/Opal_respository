using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Enums;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ValidateNominatedLearnerLogic : BaseBusinessLogic
    {
        public ValidateNominatedLearnerLogic(IUserContext userContext) : base(userContext)
        {
        }

        public ValidateLearnerResultType Execute(
            CourseEntity course,
            List<Registration> userExistedInProgressRegistrations)
        {
            if (userExistedInProgressRegistrations == null)
            {
                return ValidateLearnerResultType.Valid;
            }

            if (userExistedInProgressRegistrations.Any())
            {
                var notCompleteRegistrations = userExistedInProgressRegistrations.Where(x => x.IsNotAbleToBeNominated()).ToList();
                if (notCompleteRegistrations.Any())
                {
                    if (notCompleteRegistrations.Any(x => x.RegistrationType == RegistrationType.Nominated || x.RegistrationType == RegistrationType.AddedByCA))
                    {
                        return ValidateLearnerResultType.HasUncompleteRegistrationAddedByCA;
                    }

                    // update RegistrationType if leaner is being participant but they weren't
                    var appliedRegistrations = notCompleteRegistrations.Where(x => x.Status == RegistrationStatus.ConfirmedByCA || x.Status == RegistrationStatus.OfferConfirmed);
                    if (appliedRegistrations.Any())
                    {
                        return ValidateLearnerResultType.HasUncompleteRegistrationAddedByLearner;
                    }
                }
                else if (userExistedInProgressRegistrations.Count >= course.MaxReLearningTimes)
                {
                    return ValidateLearnerResultType.MaxReLearningTimes;
                }
            }

            return ValidateLearnerResultType.Valid;
        }
    }
}
