using System;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class CommunityInfo
    {
        public Guid CommunityId { get; set; }

        public int PostId { get; set; }

        public Guid OwnerPostId { get; set; }

        public Guid OwnerCommunityId { get; set; }

        /// <summary>
        /// Detect if the post has link.
        /// </summary>
        public bool HasLink { get; set; }

        public int NumOfMultimedia { get; set; }
    }
}
