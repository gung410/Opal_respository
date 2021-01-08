using System.Collections.Generic;
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
    public class GetSessionsByIdsQueryHandler : BaseQueryHandler<GetSessionsByIdsQuery, IEnumerable<SessionModel>>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetSessionsByIdsQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Session> readSessionRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
        }

        protected override async Task<IEnumerable<SessionModel>> HandleAsync(
            GetSessionsByIdsQuery query,
            CancellationToken cancellationToken)
        {
            if (query.SessionIds == null || !query.SessionIds.Any())
            {
                return new List<SessionModel>();
            }

            var sessions = await _readSessionRepository.GetAll()
                .Where(x => query.SessionIds.Contains(x.Id))
                .Select(x => new SessionModel(x)).ToListAsync(cancellationToken);

            return sessions;
        }
    }
}
