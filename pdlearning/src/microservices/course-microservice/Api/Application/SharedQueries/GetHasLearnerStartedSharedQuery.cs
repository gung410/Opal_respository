using System;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetHasLearnerStartedSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetHasLearnerStartedSharedQuery(IReadOnlyRepository<Registration> readRegistrationRepository)
        {
            _readRegistrationRepository = readRegistrationRepository;
        }

        public async Task<bool> ByClassRunId(Guid classRunId)
        {
            var hasLearnerStarted = await _readRegistrationRepository
                .FirstOrDefaultAsync(x => x.ClassRunId == classRunId && x.LearningStatus != LearningStatus.NotStarted);

            return hasLearnerStarted != null;
        }
    }
}
