using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetTeamMemberEventsQuery : BaseThunderQuery<List<TeamMemberEventModel>>
    {
        public GetTeamMemberEventsRequest Request { get; set; }

        public Guid ApproveOfficerId { get; set; }
    }
}
