using System.Runtime.Serialization;

namespace LearnerApp.Models.Learner
{
    public enum ResourceTypesFilter
    {
        All,
        Course,
        Microlearning,
        Content,
        [EnumMember(Value = "memberships.id")]
        MyCommunities,
        [EnumMember(Value = "createdby")]
        MyOwnCommunities,
        Community,
        LearningPath,
        LearningPathLMM,
        Form
    }
}
