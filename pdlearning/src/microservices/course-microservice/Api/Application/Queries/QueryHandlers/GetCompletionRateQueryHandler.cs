using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetCompletionRateQueryHandler : BaseQueryHandler<GetCompletionRateQuery, double>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;

        public GetCompletionRateQueryHandler(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<ClassRun> readClassrunRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readClassrunRepository = readClassrunRepository;
        }

        protected override async Task<double> HandleAsync(
            GetCompletionRateQuery query,
            CancellationToken cancellationToken)
        {
            var classrunQuery = _readClassrunRepository.GetAll()
                .Where(p => p.Id == query.ClassRunId)
                .Where(ClassRun.IsNotCancelledExpr());
            var participantsQuery = _readRegistrationRepository.GetAll()
                .Join(classrunQuery, p => p.ClassRunId, p => p.Id, (participant, classrun) => participant)
                .Where(Registration.IsParticipantExpr());

            var numberOfCompletedParticipants = await participantsQuery.Where(Registration.IsCompletedExpr()).CountAsync(cancellationToken);
            var totalParticipants = await participantsQuery.CountAsync(cancellationToken);

            if (totalParticipants == 0)
            {
                return totalParticipants;
            }

            double completionRate = (double)numberOfCompletedParticipants * 100 / totalParticipants;

            return Math.Round(completionRate, 2);
        }
    }
}
