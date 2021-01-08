using LearnerApp.Services;
using Newtonsoft.Json;

namespace LearnerApp.Common.Enum
{
    [JsonConverter(typeof(DefaultEnumConverter), (int)Other)]
    public enum ContentType
    {
        LearningContent,
        UploadedContent,
        Other
    }
}
