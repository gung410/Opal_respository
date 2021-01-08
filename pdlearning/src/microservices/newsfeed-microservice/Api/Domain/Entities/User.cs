using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Microservice.NewsFeed.Domain.Entities
{
    /// <summary>
    /// Sync from SAM module.
    /// </summary>
    public class User
    {
        public User(
            int originalUserId,
            string firstName,
            string lastName,
            string email,
            Guid extId,
            string avatarUrl,
            List<Guid> followers)
        {
            OriginalUserId = originalUserId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            ExtId = extId;
            AvatarUrl = avatarUrl;
            Followers = followers;
        }

        /// <summary>
        /// This field automatically is generated please don't create new for it.
        /// </summary>
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        /// <summary>
        /// The identifier user (integer).
        /// </summary>
        public int OriginalUserId { get; private set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// User's email.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// The identifier user (Guid).
        /// </summary>
        public Guid ExtId { get; private set; }

        /// <summary>
        /// User avatar.
        /// </summary>
        public string AvatarUrl { get; private set; }

        /// <summary>
        /// The identifier of followers.
        /// </summary>
        public List<Guid> Followers { get; private set; }

        public static Expression<Func<User, bool>> FilterByExtIdExpr(Guid extId)
        {
            return p => p.ExtId == extId;
        }

        public void Update(
            string firstName,
            string lastName,
            string emailAddress,
            string avatarUrl)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = emailAddress;
            AvatarUrl = avatarUrl;
        }

        public void AddFollower(Guid userId)
        {
            // Prevent adding the same follower
            // when data sync is triggered from CSL.
            if (Followers.Any(f => f == userId))
            {
                return;
            }

            Followers.Add(userId);
        }

        public void RemoveFollower(Guid userId)
        {
            Followers.Remove(userId);
        }
    }
}
