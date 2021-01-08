using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Helpers;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetCoursesSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public GetCoursesSharedQuery(IReadOnlyRepository<CourseEntity> readCourseRepository)
        {
            _readCourseRepository = readCourseRepository;
        }

        public async Task<List<CourseModel>> WithExpiredCourses(CancellationToken cancellationToken = default)
        {
            var courses = await _readCourseRepository.GetAll()
                .Where(x => x.ExpiredDate < Clock.Now && x.Status == CourseStatus.Published)
                .ToListAsync(cancellationToken);

            return courses.Select(p => new CourseModel(p)).ToList();
        }

        public async Task<List<CourseModel>> WithNotPublishedCourses(CancellationToken cancellationToken = default)
        {
            var courses = await _readCourseRepository.GetAll()
                .Where(CourseEntity.CanPublishValidator().IsValidExpression)
                .Where(x => x.PlanningPublishDate.HasValue && x.PlanningPublishDate.Value <= DateTimeHelper.EndOfTodayInSystemTimeZone().ToUniversalTime())
                .ToListAsync(cancellationToken);

            return courses.Select(p => new CourseModel(p)).ToList();
        }

        public async Task<List<CourseModel>> WithContentNotPublishedCourses(CancellationToken cancellationToken = default)
        {
            var courses = await _readCourseRepository.GetAll()
                .Where(CourseEntity.CanPublishContentValidator().IsValidExpression)
                .Where(x => x.PlanningPublishDate.HasValue && x.PlanningPublishDate.Value <= DateTimeHelper.EndOfTodayInSystemTimeZone().ToUniversalTime())
                .ToListAsync(cancellationToken);

            return courses.Select(p => new CourseModel(p)).ToList();
        }
    }
}
