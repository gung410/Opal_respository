using System;
using Microservice.Badge.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Domain.Entities
{
    public class TopBadgeQualifiedUser
    {
        public TopBadgeQualifiedUser(Guid badgeId, Guid userId)
        {
            BadgeId = badgeId;
            UserId = userId;
            CreatedDate = Clock.Now;
        }

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public Guid BadgeId { get; init; }

        public Guid? CommunityId { get; private set; }

        public DateTime CreatedDate { get; init; }

        public GeneralStatistic GeneralStatistic { get; private set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsDeleted { get; set; }

        public int NumOfQualifiedPost { get; set; }

        public int NumOfCreatedForum { get; set; }

        public string FullName() => (FirstName ?? string.Empty) + " " + (LastName ?? string.Empty).Trim();

        public TopBadgeQualifiedUser UpdateGeneralStatistic(GeneralStatistic generalStatistic)
        {
            this.GeneralStatistic = generalStatistic;
            return this;
        }

        public TopBadgeQualifiedUser SetCommunityId(Guid communityId)
        {
            this.CommunityId = communityId;
            return this;
        }

        public TopBadgeQualifiedUser SetNumOfQualifiedPost(int numOfQualifiedPost)
        {
            this.NumOfQualifiedPost = numOfQualifiedPost;
            return this;
        }

        public TopBadgeQualifiedUser SetNumOfCreatedForum(int numOfCreatedForum)
        {
            this.NumOfCreatedForum = numOfCreatedForum;
            return this;
        }

        public TopBadgeQualifiedUser UpdateUserInfo(string firstName, string lastName, string email)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            return this;
        }
    }
}
