using System;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class CommunityChangedMessage
    {
        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Name of the community.
        /// </summary>
        public string Name { get; init; }
    }
}
