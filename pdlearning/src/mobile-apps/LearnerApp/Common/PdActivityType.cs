using System.Runtime.Serialization;

namespace LearnerApp.Common
{
    public enum PdActivityType
    {
        [EnumMember(Value = "course")]
        Courses,
        [EnumMember(Value = "microlearning")]
        MicrolearningUnits,
        DigitalContent,
        [EnumMember(Value = "community")]
        Communities,
        [EnumMember(Value = "learningpath")]
        LearningPaths,
        Bookmarks
    }
}
