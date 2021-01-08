#pragma warning disable SA1402 // File may only contain a single type
using System;

namespace Microservice.NewsFeed.Application.Consumers.Messages
{
    public class CommunityMembershipMessage
    {
        public UserIdentity User { get; set; }

        public Community Community { get; set; }
    }

    public class Community
    {
        /// <summary>
        /// Considered as the identifier community.
        /// </summary>
        public Guid Id { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
