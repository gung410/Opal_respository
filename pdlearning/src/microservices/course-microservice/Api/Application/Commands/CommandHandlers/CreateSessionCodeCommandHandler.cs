using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CreateSessionCodeCommandHandler : BaseCommandHandler<CreateSessionCodeCommand>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly SessionCudLogic _sessionCudLogic;

        public CreateSessionCodeCommandHandler(
           IUnitOfWorkManager unitOfWorkManager,
           IReadOnlyRepository<ClassRun> readClassRunRepository,
           IReadOnlyRepository<Session> readSessionRepository,
           IUserContext userContext,
           SessionCudLogic sessionCudLogic,
           IAccessControlContext<CourseUser> accessControlContext,
           GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readSessionRepository = readSessionRepository;
            _sessionCudLogic = sessionCudLogic;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
        }

        protected override async Task HandleAsync(CreateSessionCodeCommand command, CancellationToken cancellationToken)
        {
            var aggregatedSession = await _getAggregatedSessionSharedQuery.ById(command.SessionId, cancellationToken);

            if (!string.IsNullOrEmpty(aggregatedSession.Session.SessionCode))
            {
                return;
            }

            EnsureBusinessLogicValid(aggregatedSession.Course.ValidateNotArchived());

            var existedSessionCodes = await _readSessionRepository
                .GetAll()
                .Where(x => x.ClassRunId == aggregatedSession.Session.ClassRunId && x.SessionCode != null)
                .Select(x => x.SessionCode)
                .ToListAsync(cancellationToken);

            var classRunCode = await _readClassRunRepository.GetAll()
                .Where(x => x.Id == aggregatedSession.Session.ClassRunId && x.ClassRunCode != null)
                .Select(x => x.ClassRunCode)
                .FirstOrDefaultAsync(cancellationToken);

            aggregatedSession.Session.SessionCode = RandomHelper.GenerateUniqueString(Session.SessionCodeLength, existedSessionCodes, classRunCode);

            await _sessionCudLogic.Update(aggregatedSession, cancellationToken);
        }
    }
}
