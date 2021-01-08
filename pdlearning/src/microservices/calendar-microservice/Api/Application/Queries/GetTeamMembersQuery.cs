using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetTeamMembersQuery : BaseThunderQuery<List<TeamMemberModel>>
    {
        public Guid ApproveOfficerId { get; set; }
    }
}
