using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetMySharedTeamsQuery : BaseThunderQuery<List<SharedTeamModel>>
    {
        public Guid UserId { get; set; }
    }
}
