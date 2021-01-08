#pragma warning disable SA1402 // File may only contain a single type
using System;

namespace Microservice.NewsFeed.Application.Consumers.Messages
{
    public class UserFollowMessage
    {
        public UserIdentity User { get; set; }

        public Target Target { get; set; }

        /// <summary>
        /// Include user, post, wiki, forum.
        /// </summary>
        public string TargetType { get; set; }

        public bool IsUserFollowUser()
        {
            return TargetType == "user";
        }
    }

    public class Target
    {
        public int Id { get; set; }

        /// <summary>
        /// Considered as the UserId.
        /// </summary>
        public Guid Guid { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
