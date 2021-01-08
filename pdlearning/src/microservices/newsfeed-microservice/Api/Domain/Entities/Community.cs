using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Microservice.NewsFeed.Domain.Entities
{
    /// <summary>
    /// Sync from the CSL module.
    /// </summary>
    public class Community
    {
        public Community(
            Guid communityId,
            string description,
            string communityName,
            string communityThumbnail,
            string visibility,
            string url,
            Guid? mainCommunityId,
            string status,
            Guid createdBy,
            Guid? updatedBy,
            DateTime createdAt,
            DateTime? updatedAt)
        {
            Url = url;
            Status = status;
            CreatedBy = createdBy;
            UpdatedBy = updatedBy;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Visibility = visibility;
            CommunityId = communityId;
            Description = description;
            CommunityName = communityName;
            MainCommunityId = mainCommunityId;
            CommunityThumbnail = communityThumbnail;
        }

        /// <summary>
        /// This field automatically is generated please don't create new for it.
        /// </summary>
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid CommunityId { get; private set; }

        /// <summary>
        /// Description of the community.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Name of the community.
        /// </summary>
        public string CommunityName { get; private set; }

        /// <summary>
        /// Thumbnail of the community.
        /// </summary>
        public string CommunityThumbnail { get; private set; }

        /// <summary>
        /// Community url.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid? MainCommunityId { get; private set; }

        /// <summary>
        /// The identifier community owner.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// The identifier community owner.
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// Date of community creation.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date of community change.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Members of the community.
        /// </summary>
        public List<Guid> Membership { get; private set; } = new List<Guid>();

        public string Status { get; private set; }

        public string Visibility { get; private set; }

        public static Expression<Func<Community, bool>> FilterByIdExpr(Guid communityId)
        {
            return p => p.CommunityId == communityId;
        }

        public void Update(
            string communityName,
            string communityThumbnail,
            string description,
            string url,
            string visibility,
            string status)
        {
            CommunityName = communityName;
            CommunityThumbnail = communityThumbnail;
            Description = description;
            Url = url;
            Visibility = visibility;
            Status = status;
        }

        /// <summary>
        /// Add member to the community.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        public void AddMembership(Guid userId)
        {
            // Prevent adding the same community member
            // when data sync is triggered from CSL.
            if (Membership.Any(m => m == userId))
            {
                return;
            }

            Membership.Add(userId);
        }

        /// <summary>
        /// Remove member from the community.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        public void RemoveMembership(Guid userId)
        {
            Membership.Remove(userId);
        }

        /// <summary>
        /// Check the community name has changed.
        /// </summary>
        /// <param name="communityName">Name of the community.</param>
        /// <returns>Returns true if the old community name not equal to the new one,
        /// otherwise false.</returns>
        public bool IsDifferentCommunityName(string communityName)
        {
            return CommunityName != communityName;
        }

        /// <summary>
        /// Check the community description has changed.
        /// </summary>
        /// <param name="communityDescription">Description of the community.</param>
        /// <returns>Returns true if the old community description not equal to the new one,
        /// otherwise false.</returns>
        public bool IsDifferentCommunityDescription(string communityDescription)
        {
            return Description != communityDescription;
        }
    }
}
