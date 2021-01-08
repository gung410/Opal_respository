using System;
using System.Collections.Generic;
using Microservice.Badge.Domain.Enums;

namespace Microservice.Badge.Application.Models
{
    public class UserStatisticEnumerationModel
    {
        public Guid UserId { get; set; }

        public IEnumerable<ActivityEnumerationModel> Activities { get; set; }
    }

    public class ActivityEnumerationModel
    {
        public ActivityType Type { get; set; }

        public int Count { get; set; }
    }
}
