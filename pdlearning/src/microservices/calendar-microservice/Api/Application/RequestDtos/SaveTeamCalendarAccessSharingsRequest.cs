using System;
using System.Collections.Generic;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class SaveTeamCalendarAccessSharingsRequest
    {
        /// <summary>
        /// Share or unshare Team Calendar access action.
        /// </summary>
        public SaveTeamAccessAction Action { get; set; }

        /// <summary>
        /// The IDs of users will be granted or revoking access based on <see cref="SaveTeamAccessAction"/>.
        /// </summary>
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
}
