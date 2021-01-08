using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetCourseInvolvedUsersSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;

        public GetCourseInvolvedUsersSharedQuery(IReadOnlyRepository<CourseUser> readUserRepository)
        {
            _readUserRepository = readUserRepository;
        }

        public async Task<IEnumerable<CourseUser>> Execute(List<CourseEntity> courses, CancellationToken cancellationToken)
        {
            var allCoursesUserIds = courses
                .Select(p => p.GetAllInvoledUserIds())
                .SelectMany(userIds => userIds)
                .Distinct();

            var result = await _readUserRepository
                .GetAll()
                .Where(p => allCoursesUserIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
