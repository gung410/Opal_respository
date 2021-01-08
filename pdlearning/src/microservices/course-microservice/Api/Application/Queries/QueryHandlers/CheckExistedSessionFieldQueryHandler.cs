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
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class CheckExistedSessionFieldQueryHandler : BaseQueryHandler<CheckExistedSessionFieldQuery, bool>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public CheckExistedSessionFieldQueryHandler(
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
        }

        protected override Task<bool> HandleAsync(CheckExistedSessionFieldQuery query, CancellationToken cancellationToken)
        {
            return _readSessionRepository
                .GetAll()
                .Where(x => x.Id != query.SessionId && x.ClassRunId == query.ClassRunId && (
                    x.StartDateTime.Value <= query.SessionDate.EndOfDateInSystemTimeZone().ToUniversalTime() &&
                    x.StartDateTime.Value >= query.SessionDate.StartOfDateInSystemTimeZone().ToUniversalTime()))
                .AnyAsync(cancellationToken);
        }
    }
}
