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
    public class GetAggregatedAnnouncementSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;
        private readonly IReadOnlyRepository<CourseUser> _userRepository;
        private readonly IReadOnlyRepository<Registration> _registrationRepository;

        public GetAggregatedAnnouncementSharedQuery(
            IReadOnlyRepository<CourseUser> userRepository,
            IReadOnlyRepository<ClassRun> readClassrunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> registrationRepository)
        {
            _userRepository = userRepository;
            _readClassrunRepository = readClassrunRepository;
            _readCourseRepository = readCourseRepository;
            _registrationRepository = registrationRepository;
        }

        public async Task<List<AnnouncementAggregatedEntityModel>> FullByQuery(IQueryable<Announcement> query, CancellationToken cancellationToken)
        {
            var announcementWithCourseClassRuns = await query
                .Join(_readCourseRepository.GetAll(), p => p.CourseId, p => p.Id, (announcement, course) => new { announcement, course })
                .Join(_readClassrunRepository.GetAll(), p => p.announcement.ClassrunId, p => p.Id, (group1, classRun) => new { group1.announcement, group1.course, classRun })
                .ToListAsync(cancellationToken);

            var allParticipantRegistrationIds =
                announcementWithCourseClassRuns.SelectMany(p => p.announcement.Participants).Distinct();
            var allParticipants = await _registrationRepository.GetAll().Where(p => allParticipantRegistrationIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            var allParticipantUserIds =
                allParticipants.Select(p => p.Value.UserId).Distinct();
            var allParticipantUsers = await _userRepository.GetAll().Where(p => allParticipantUserIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p, cancellationToken);
            var allParticipantToUserDic =
                allParticipants.ToDictionary(p => p.Key, p => allParticipantUsers[p.Value.UserId]);

            return announcementWithCourseClassRuns
                .Select(p => AnnouncementAggregatedEntityModel
                    .New(p.announcement)
                    .WithClassRun(p.classRun)
                    .WithCourse(p.course)
                    .WithParticipants(p.announcement.Participants.Select(participantId => allParticipants[participantId]).ToList())
                    .WithParticipantUsers(p.announcement.Participants.Select(participantId => allParticipantToUserDic[participantId]).ToList()))
                .ToList();
        }

        public async Task<AnnouncementAggregatedEntityModel> FullByAnnouncement(Announcement announcement, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(announcement.CourseId);
            var classrun = await _readClassrunRepository.GetAsync(announcement.ClassrunId);

            var allParticipantRegistrationIds = announcement.Participants;
            var allParticipants = await _registrationRepository.GetAll().Where(p => allParticipantRegistrationIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            var allParticipantUserIds =
                allParticipants.Select(p => p.Value.UserId).Distinct();
            var allParticipantUsers = await _userRepository.GetAll().Where(p => allParticipantUserIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p, cancellationToken);
            var allParticipantToUserDic =
                allParticipants.ToDictionary(p => p.Key, p => allParticipantUsers[p.Value.UserId]);

            return AnnouncementAggregatedEntityModel
                .New(announcement)
                .WithClassRun(classrun)
                .WithCourse(course)
                .WithParticipants(announcement.Participants.Select(participantId => allParticipants[participantId]).ToList())
                .WithParticipantUsers(announcement.Participants.Select(participantId => allParticipantToUserDic[participantId]).ToList());
        }
    }
}
