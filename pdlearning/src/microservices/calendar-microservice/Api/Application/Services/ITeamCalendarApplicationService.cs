using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;

namespace Microservice.Calendar.Application.Services
{
    /// <summary>
    /// Services for team calendar.
    /// Note:
    /// PAO - Primary Approval Officer.
    /// AAO - Alternative Approval Officer.
    /// </summary>
    public interface ITeamCalendarApplicationService
    {
        /// <summary>
        /// Get learner's overviews of PAO or AAO.
        /// </summary>
        /// <param name="approveOfficerId">PAO or AAO ID.</param>
        /// <param name="request">Request payload.</param>
        /// <returns>Overviews of learners.</returns>
        Task<List<TeamMemberEventOverviewModel>> GetTeamMemberEventOverview(Guid approveOfficerId, GetTeamMemberEventOverviewRequest request);

        /// <summary>
        /// Get course class runs of a learner by learner's ID.
        /// </summary>
        /// <param name="request">Request payload.</param>
        /// <param name="approveOfficerId">PAO or AAO ID.</param>
        /// <returns>Course class runs of a learner.</returns>
        Task<List<TeamMemberEventModel>> GetTeamMemberEvents(GetTeamMemberEventsRequest request, Guid approveOfficerId);

        /// <summary>
        /// Get learner's of PAO or AAO.
        /// </summary>
        /// <param name="approveOfficerId">PAO or AAO ID.</param>
        /// <returns>List learner.</returns>
        Task<List<TeamMemberModel>> GetTeamMembers(Guid approveOfficerId);
    }
}
