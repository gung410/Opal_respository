using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetSharedTeamMemberEventsQuery : BaseThunderQuery<List<TeamMemberEventModel>>
    {
        public GetSharedTeamMemberEventsRequest Request { get; set; }

        public Guid SharedWithUserId { get; set; }
    }
}
