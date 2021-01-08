using System;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.Models
{
    public class UserStatisticModel
    {
        public UserStatisticModel()
        {
        }

        public UserStatisticModel(int year, GeneralStatistic statistic)
        {
            Year = year;
            ExecutedDate = statistic.ExecutedDate;
            NumOfPost = statistic.NumOfPost;
            NumOfLikePost = statistic.NumOfLikePost;
            NumOfFollowCommunity = statistic.NumOfFollowCommunity;
            NumOfPostResponding = statistic.NumOfPostResponding;
            NumOfForward = statistic.NumOfForward;
            NumOfReflection = statistic.NumOfReflection;
            NumOfSharedReflection = statistic.NumOfSharedReflection;
            NumOfCompletedMLU = statistic.NumOfCompletedMLU;
            NumOfCompletedDigitalResources = statistic.NumOfCompletedDigitalResources;
            NumOfCompletedElearning = statistic.NumOfCompletedElearning;
            NumOfCreatedLearningPath = statistic.NumOfCreatedLearningPath;
            NumOfSharedLearningPath = statistic.NumOfSharedLearningPath;
            NumOfBookmarkedLearningPath = statistic.NumOfBookmarkedLearningPath;
            NumOfCreatedMLU = statistic.NumOfCreatedMLU;
        }

        public int Year { get; init; }

        public DateTime ExecutedDate { get; set; }

        public int NumOfPost { get; set; }

        public int NumOfLikePost { get; set; }

        public int NumOfFollowCommunity { get; set; }

        public int NumOfPostResponding { get; set; }

        public int NumOfForward { get; set; }

        public int NumOfReflection { get; set; }

        public int NumOfSharedReflection { get; set; }

        public int NumOfCompletedMLU { get; set; }

        public int NumOfCompletedDigitalResources { get; set; }

        public int NumOfCompletedElearning { get; set; }

        public int NumOfCreatedLearningPath { get; init; }

        public int NumOfSharedLearningPath { get; init; }

        public int NumOfBookmarkedLearningPath { get; init; }

        public int NumOfCreatedMLU { get; init; }

        public int Total { get; set; }
    }
}
