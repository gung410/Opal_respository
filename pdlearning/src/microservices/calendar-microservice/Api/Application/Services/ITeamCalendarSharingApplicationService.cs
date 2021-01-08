using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Calendar.Application.Services
{
    public interface ITeamCalendarSharingApplicationService
    {
        /// <summary>
        /// Gets the teams (Team Calendars) which are shared within the user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>Shared teams within the user.</returns>
        Task<List<SharedTeamModel>> GetMySharedTeams(Guid userId);

        /// <summary>
        /// Gets shared learner's overviews.
        /// </summary>
        /// <param name="request">Request payload.</param>
        /// <param name="currentUserId">The ID of user who is shared with.</param>
        /// <returns>Learner's overviews.</returns>
        Task<List<TeamMemberEventOverviewModel>> GetSharedTeamMemberEventOverview(
            GetSharedTeamMemberEventOverviewRequest request,
            Guid currentUserId);

        /// <summary>
        /// Gets the shared team members that have shared within the user.
        /// </summary>
        /// <param name="accessShareId">The access share ID (<see cref="TeamAccessSharing"/> ID).</param>
        /// <param name="userId">The user's ID.</param>
        /// <returns>Shared team members within the access share key.</returns>
        Task<List<TeamMemberModel>> GetSharedTeamMembers(Guid accessShareId, Guid userId);

        /// <summary>
        /// Gets events of shared team member (who is shared within the access share ID to the user).
        /// </summary>
        /// <param name="request">The request parameter.</param>
        /// <param name="currentUserId">The ID of user who is shared with.</param>
        /// <returns>Events of shared team member.</returns>
        Task<List<TeamMemberEventModel>> GetSharedTeamMemberEvents(GetSharedTeamMemberEventsRequest request, Guid currentUserId);

        /// <summary>
        /// Shares Team Calendar access with users.
        /// </summary>
        /// <param name="request">Access sharing request.</param>
        /// <param name="ownerId">The ID of Approval Officer who is sharing.</param>
        /// <returns>Task.</returns>
        Task SaveCalendarAccessSharings(SaveTeamCalendarAccessSharingsRequest request, Guid ownerId);

        /// <summary>
        /// Get access sharings of users.
        /// </summary>
        /// <param name="ownerId">The Approval Officer.</param>
        /// <param name="request"><see cref="PagedResultRequestDto"/> used to paginate the result items.</param>
        /// <returns>Paged access sharings of users.</returns>
        Task<PagedResultDto<UserAccessSharingModel>> GetCalendarAccessSharings(Guid ownerId, PagedResultRequestDto request);
    }
}
