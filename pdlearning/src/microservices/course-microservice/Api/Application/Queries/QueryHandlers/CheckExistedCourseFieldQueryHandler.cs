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
    public class CheckExistedCourseFieldQueryHandler : BaseQueryHandler<CheckExistedCourseFieldQuery, bool>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public CheckExistedCourseFieldQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
        }

        protected override Task<bool> HandleAsync(CheckExistedCourseFieldQuery query, CancellationToken cancellationToken)
        {
            return _readCourseRepository
                .GetAll()
                .Where(x => x.Id != query.CourseId && ((!string.IsNullOrWhiteSpace(query.ExternalCode) && x.ExternalCode == query.ExternalCode)
                        || (!string.IsNullOrWhiteSpace(query.CourseCode) && x.CourseCode == query.CourseCode)))
                .AnyAsync(cancellationToken);
        }
    }
}
