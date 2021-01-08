using System.Collections.Generic;
using Microservice.Badge.Domain.Enums;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Domain.ValueObjects
{
    /// <summary>
    /// This class define group statistic information which is executed from specific date.
    /// </summary>
    public class GeneralStatistic : BaseStatistic
    {
        public GeneralStatistic(IReadOnlyDictionary<ActivityType, int> dictUserActivities)
        {
            var numOfPostCommunity = dictUserActivities.GetValueOrDefault(ActivityType.PostCommunity);
            var numOfPostUserWall = dictUserActivities.GetValueOrDefault(ActivityType.PostUserWall);
            ExecutedDate = Clock.Now;
            NumOfPost = numOfPostCommunity + numOfPostUserWall;
            NumOfLikePost = dictUserActivities.GetValueOrDefault(ActivityType.LikePost);
            NumOfFollowCommunity = dictUserActivities.GetValueOrDefault(ActivityType.FollowCommunity);
            NumOfPostResponding = dictUserActivities.GetValueOrDefault(ActivityType.CommentOthersPost);
            NumOfForward = dictUserActivities.GetValueOrDefault(ActivityType.Forward);
            NumOfReflection = dictUserActivities.GetValueOrDefault(ActivityType.CreateReflection);
            NumOfSharedReflection = dictUserActivities.GetValueOrDefault(ActivityType.CreateSharedReflection);
            NumOfCompletedDigitalResources = dictUserActivities.GetValueOrDefault(ActivityType.CompleteDigitalResources);
            NumOfCompletedElearning = dictUserActivities.GetValueOrDefault(ActivityType.CompleteElearning);
            NumOfCompletedMLU = dictUserActivities.GetValueOrDefault(ActivityType.CompleteMLU);
            NumOfCreatedLearningPath = dictUserActivities.GetValueOrDefault(ActivityType.CreatedLearningPath);
            NumOfSharedLearningPath = dictUserActivities.GetValueOrDefault(ActivityType.SharedLearningPath);
            NumOfBookmarkedLearningPath = dictUserActivities.GetValueOrDefault(ActivityType.BookmarkedLearningPath);
            NumOfCreatedMLU = dictUserActivities.GetValueOrDefault(ActivityType.CreatedMLU);
            ActiveContributorActivityTotal = NumOfCreatedLearningPath + NumOfSharedLearningPath + NumOfBookmarkedLearningPath + NumOfCreatedMLU;
            ReflectiveActivityTotal = NumOfReflection + NumOfSharedReflection;
            Total = NumOfPost + NumOfLikePost + NumOfFollowCommunity + NumOfPostResponding + NumOfForward +
                    NumOfReflection + NumOfSharedReflection + NumOfCompletedDigitalResources + NumOfCompletedElearning +
                    NumOfCompletedMLU + NumOfCreatedLearningPath + NumOfSharedLearningPath + NumOfBookmarkedLearningPath + NumOfCreatedMLU;
        }

        public int NumOfPost { get; init; }

        public int NumOfLikePost { get; init; }

        public int NumOfFollowCommunity { get; init; }

        public int NumOfCreatedLearningPath { get; init; }

        public int NumOfSharedLearningPath { get; init; }

        public int NumOfBookmarkedLearningPath { get; init; }

        public int NumOfCreatedMLU { get; init; }

        /// <summary>
        /// Comment to others posts.
        /// </summary>
        public int NumOfPostResponding { get; init; }

        public int NumOfForward { get; init; }

        public int NumOfReflection { get; init; }

        public int NumOfSharedReflection { get; init; }

        public int NumOfCompletedMLU { get; init; }

        public int NumOfCompletedDigitalResources { get; init; }

        public int NumOfCompletedElearning { get; init; }

        public int ReflectiveActivityTotal { get; init; }

        public int ActiveContributorActivityTotal { get; init; }

        public int Total { get; init; }
    }
}
