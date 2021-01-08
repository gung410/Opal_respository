using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetSessionCodeByIdQueryHandler : BaseQueryHandler<GetSessionCodeByIdQuery, SessionModel>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetSessionCodeByIdQueryHandler(
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
        }

        protected override async Task<SessionModel> HandleAsync(
            GetSessionCodeByIdQuery query,
            CancellationToken cancellationToken)
        {
            EnsureValidPermission(Session.HasGetSessionCodePermission(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));

            var session = await _readSessionRepository.GetAll()
                .Where(x => x.Id == query.SessionId)
                .FirstOrDefaultAsync(cancellationToken);

            var sessionModel = new SessionModel(session)
            {
                SessionCode = session.SessionCode
            };

            return sessionModel;
        }
    }
}
