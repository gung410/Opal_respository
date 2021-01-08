using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Models
{
    public class CommunityEventModel : EventModel
    {
        public Guid CommunityId { get; set; }

        public Guid? UserId { get; set; }

        public string CommunityTitle { get; set; }

        public CommunityEventPrivacy? CommunityEventPrivacy { get; set; }
    }
}
