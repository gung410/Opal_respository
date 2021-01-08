using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetSessionByIdQueryHandler : BaseQueryHandler<GetSessionByIdQuery, SessionModel>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetSessionByIdQueryHandler(
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
        }

        protected override async Task<SessionModel> HandleAsync(
            GetSessionByIdQuery query,
            CancellationToken cancellationToken)
        {
            var session = await _readSessionRepository.GetAsync(query.Id);

            return new SessionModel(session);
        }
    }
}
