using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries.Abstractions;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.SharedQueries
{
    public class TeamMemberEventOverviewSharedQuery : BaseSharedQuery
    {
        private readonly IRepository<CalendarUser> _calendarUserRepository;
        private readonly IRepository<UserPersonalEvent> _userPersonalEventRepository;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<TeamAccessSharing> _teamAccessSharingRepository;
        private readonly IRepository<Course> _courseRepository;

        public TeamMemberEventOverviewSharedQuery(
            IRepository<CalendarUser> calendarUserRepository,
            IRepository<UserPersonalEvent> userPersonalEventRepository,
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<TeamAccessSharing> teamAccessSharingRepository,
            IRepository<Course> courseRepository)
        {
            _calendarUserRepository = calendarUserRepository;
            _userPersonalEventRepository = userPersonalEventRepository;
            _personalEventRepository = personalEventRepository;
            _teamAccessSharingRepository = teamAccessSharingRepository;
            _courseRepository = courseRepository;
        }

        public async Task<Guid> GetOwnerIdOfUserByAccessSharingId(Guid accessSharingId, Guid userId)
        {
            var accessSharing = await _teamAccessSharingRepository
               .FirstOrDefaultAsync(s => s.Id == accessSharingId && s.UserId == userId);

            return accessSharing != null ? accessSharing.OwnerId : Guid.Empty;
        }

        /// <summary>
        /// Get list member belong to Approve Offiver who has Active status.
        /// </summary>
        /// <param name="approveOfficerId">Alternative/Primary Approve Office Id.</param>
        /// <param name="cancellationToken">.</param>
        /// <returns>List member.</returns>
        public Task<List<TeamMemberModel>> GetTeamMember(Guid approveOfficerId, CancellationToken cancellationToken)
        {
            return _calendarUserRepository
                .GetAll()
                .ByStatus(UserStatus.Active)
                .BelongsToApprovalOfficer(approveOfficerId)
                .Select(p => new TeamMemberModel
                {
                    LearnerId = p.Id,
                    LearnerName = p.FullName().Trim()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TeamMemberEventOverviewModel>> GetTeamMemberEventOverview(Guid approveOfficerId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken)
        {
            var learnersQuery = _calendarUserRepository
                .GetAll()
                .ByStatus(UserStatus.Active)
                .BelongsToApprovalOfficer(approveOfficerId);

            var memberEventOverviews = await _personalEventRepository
                .GetAll()
                .HasSource(CalendarEventSource.CourseClassRun)
                .GetAvailableEvents()
                .OverlapsDateTimeRange(rangeStart, rangeEnd)
                .GetMemberEventOverviews(learnersQuery, _userPersonalEventRepository.GetAll().GetAcceptedEvents())
                .ToListAsync(cancellationToken);

            var memberOverviewsResultList = memberEventOverviews
                 .GroupBy(o => o.UserId)
                 .Select(overviewGroup =>
                     new TeamMemberEventOverviewModel
                     {
                         Id = overviewGroup.Key,
                         Title = overviewGroup.First().FullName,
                         StartAt = overviewGroup.Min(o => o.StartAt),
                         EndAt = overviewGroup.Max(o => o.EndAt)
                     })
                 .ToList();

            return memberOverviewsResultList;
        }

        public Task<List<TeamMemberEventModel>> GetTeamMemberEvents(Guid learnerId, Guid approveOfficerId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken)
        {
            var memberEvents = _personalEventRepository
                .GetAll()
                .HasSource(CalendarEventSource.CourseClassRun)
                .GetAvailableEvents()
                .OverlapsDateTimeRange(rangeStart, rangeEnd)
                .OfUser(
                    userId: learnerId,
                    approvalOfficerId: approveOfficerId,
                    userRepo: _calendarUserRepository,
                    userPersonalEventRepo: _userPersonalEventRepository)
                .MapToTeamMemberEvents(
                    parentId: learnerId,
                    courseRepo: _courseRepository);

            return memberEvents.ToListAsync(cancellationToken);
        }
    }
}
