using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetAggregatedAttendanceTrackingSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetAggregatedAttendanceTrackingSharedQuery(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassrunRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository)
        {
            _readCourseRepository = readCourseRepository;
            _readClassrunRepository = readClassrunRepository;
            _readRegistrationRepository = readRegistrationRepository;
        }

        public async Task<List<AttendanceTrackingAggregatedEntityModel>> FullByQuery(IQueryable<AttendanceTracking> query, CancellationToken cancellationToken)
        {
            var attendanceTrackingToCourse = await query
                .Join(
                    _readRegistrationRepository.GetAll(),
                    p => p.RegistrationId,
                    p => p.Id,
                    (attendanceTracking, registration) => new { attendanceTracking, registration })
                .Join(
                    _readClassrunRepository.GetAll(),
                    p => p.registration.ClassRunId,
                    p => p.Id,
                    (gj, classrun) => new { gj.registration, gj.attendanceTracking, classrun })
                .Join(
                    _readCourseRepository.GetAll(),
                    p => p.registration.CourseId,
                    p => p.Id,
                    (gj, course) => new { gj.attendanceTracking, gj.classrun, gj.registration, course })
                .ToListAsync(cancellationToken);

            return attendanceTrackingToCourse
                .Select(
                    p => AttendanceTrackingAggregatedEntityModel
                        .New(p.attendanceTracking)
                        .WithRegistration(p.registration)
                        .WithClassRun(p.classrun)
                        .WithCourse(p.course))
                .ToList();
        }

        public async Task<List<AttendanceTrackingAggregatedEntityModel>> WithRegistrationByQuery(
            IQueryable<AttendanceTracking> query,
            CancellationToken cancellationToken)
        {
            var attendanceTrackingToCourse = await query
                .Join(
                    _readRegistrationRepository.GetAll(),
                    p => p.RegistrationId,
                    p => p.Id,
                    (attendanceTracking, registration) => new { attendanceTracking, registration })
                .ToListAsync(cancellationToken);

            return attendanceTrackingToCourse
                .Select(
                    p => AttendanceTrackingAggregatedEntityModel
                        .New(p.attendanceTracking)
                        .WithRegistration(p.registration))
                .ToList();
        }
    }
}
