using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetTeamMemberEventOverviewQuery : BaseThunderQuery<List<TeamMemberEventOverviewModel>>
    {
        public Guid ApproveOfficerId { get; set; }

        public DateTime RangeStart { get; set; }

        public DateTime RangeEnd { get; set; }
    }
}
