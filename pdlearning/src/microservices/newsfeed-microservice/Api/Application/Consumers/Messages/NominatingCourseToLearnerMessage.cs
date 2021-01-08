using System;

// ReSharper disable once CheckNamespace
namespace Microservice.NewsFeed.Application.Consumers.Messages.NominatingCourseToLearner
{
    public class NominatingCourseToLearnerMessage
    {
        public NominatingResult Result { get; set; }
    }

    public class NominatingResult
    {
        public ObjectiveInfo ObjectiveInfo { get; set; }

        public AdditionalProperties AdditionalProperties { get; set; }

        public bool IsCourseRecommended()
        {
            return AdditionalProperties.Type == "recommended";
        }
    }

    public class AdditionalProperties
    {
        public string Type { get; set; }

        public Guid CourseId { get; set; }
    }

    public class ObjectiveInfo
    {
        public string Email { get; set; }

        public string Avatar { get; set; }

        public UserIdentity Identity { get; set; }

        public string Name { get; set; }

        public object Description { get; set; }
    }

    public class UserIdentity
    {
        public Guid ExtId { get; set; }
    }
}
