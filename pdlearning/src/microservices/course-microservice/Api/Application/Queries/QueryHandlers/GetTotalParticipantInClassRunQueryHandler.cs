using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetTotalParticipantInClassRunQueryHandler : BaseQueryHandler<GetTotalParticipantInClassRunQuery, IEnumerable<TotalParticipantInClassRunModel>>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetTotalParticipantInClassRunQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readRegistrationRepository = readRegistrationRepository;
        }

        protected override async Task<IEnumerable<TotalParticipantInClassRunModel>> HandleAsync(GetTotalParticipantInClassRunQuery query, CancellationToken cancellationToken)
        {
            var participantTotalPerClass = await _readRegistrationRepository
                .GetAll()
                .Where(x => query.ClassRunIds.Contains(x.ClassRunId))
                .Where(Registration.IsParticipantExpr())
                .GroupBy(x => x.ClassRunId)
                .Select(x => new TotalParticipantInClassRunModel
                {
                    ClassRunId = x.Key,
                    ParticipantTotal = x.Count()
                })
                .ToDictionaryAsync(p => p.ClassRunId, p => p, cancellationToken);

            return query.ClassRunIds.Select(p => participantTotalPerClass.GetValueOrDefault(p, new TotalParticipantInClassRunModel(p)));
        }

        protected override async Task<Validation<GetTotalParticipantInClassRunQuery>> ValidateQuery(GetTotalParticipantInClassRunQuery query, CancellationToken cancellationToken)
        {
            var existedClassRunIds = await _readClassRunRepository
                .GetAll()
                .Where(p => query.ClassRunIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (existedClassRunIds.Count != query.ClassRunIds.Count)
            {
                return Validation<GetTotalParticipantInClassRunQuery>.Invalid("Some classRunId is invalid");
            }

            return Validation<GetTotalParticipantInClassRunQuery>.Valid(query);
        }
    }
}
