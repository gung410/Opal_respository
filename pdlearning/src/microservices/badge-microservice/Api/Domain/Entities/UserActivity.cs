using System;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Domain.Entities
{
    /// <summary>
    /// This entity is used for syncing activity of user when they do one thing on Opal2.
    /// </summary>
    public class UserActivity
    {
        public UserActivity(Guid userId, DateTime? activityDate, object sourceId, ActivityType activityType)
        {
            ActivityDate = activityDate ?? Clock.Now;
            UserId = userId;
            Type = activityType;
            SourceId = sourceId.ToString();
        }

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; init; }

        /// <summary>
        /// Id of user who make this activity.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        /// It's type of action. For example: create micro learning course maybe "CreateCourse".
        /// </summary>
        public ActivityType Type { get; init; }

        /// <summary>
        /// Id of one thing which user action.
        /// Example: If user create course,them SourceId will be CourseId.
        /// We define string because some id of entities come from other project may be integer or string.
        /// </summary>
        public string SourceId { get; init; }

        /// <summary>
        /// Date which user make this action.
        ///  </summary>
        public DateTime ActivityDate { get; init; }

        /// <summary>
        /// Contain Community information when this activity from CSL Module.
        /// </summary>
        public CommunityInfo CommunityInfo { get; set; }

        /// <summary>
        /// Contain EPortfolio information when this activity is received from EPortfolio module.
        /// </summary>
        public EPortfolioInfo EPortfolioInfo { get; set; }

        public CourseInfo CourseInfo { get; set; }

        public UserActivity WithPortfolioInfo(EPortfolioInfo ePortfolioInfo)
        {
            this.EPortfolioInfo = ePortfolioInfo;
            return this;
        }

        public UserActivity WithCommunityInfo(CommunityInfo communityInfo)
        {
            this.CommunityInfo = communityInfo;
            return this;
        }

        public UserActivity WithCourseInfo(CourseInfo courseInfo)
        {
            this.CourseInfo = courseInfo;
            return this;
        }
    }
}
