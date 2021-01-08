using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ProcessJustLearningFailedParticipantLogic : BaseBusinessLogic
    {
        private readonly SendLearningRecordEventLogic _learningRecordEventLogic;
        private readonly RegistrationECertificateCudLogic _registrationECertificateCudLogic;

        public ProcessJustLearningFailedParticipantLogic(
            SendLearningRecordEventLogic learningRecordEventLogic,
            RegistrationECertificateCudLogic registrationECertificateCudLogic,
            IUserContext userContext) : base(userContext)
        {
            _learningRecordEventLogic = learningRecordEventLogic;
            _registrationECertificateCudLogic = registrationECertificateCudLogic;
        }

        public async Task<List<Registration>> Execute(List<Registration> registrations, CancellationToken cancellationToken = default)
        {
            var failedRegistrations = registrations.Where(x => x.IsFailed()).ToList();

            await _learningRecordEventLogic.ByRegistrations(failedRegistrations, cancellationToken);

            await _registrationECertificateCudLogic.DeleteManyAsync(failedRegistrations.Select(p => p.Id));

            return failedRegistrations;
        }
    }
}
