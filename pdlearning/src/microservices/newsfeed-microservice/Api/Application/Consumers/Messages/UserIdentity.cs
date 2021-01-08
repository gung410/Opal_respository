using System;

namespace Microservice.NewsFeed.Application.Consumers.Messages
{
    public class UserIdentity
    {
        /// <summary>
        /// Considered as the UserId.
        /// </summary>
        public Guid Guid { get; set; }
    }
}
