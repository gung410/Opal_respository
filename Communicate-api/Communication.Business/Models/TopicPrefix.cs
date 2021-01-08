namespace Communication.Business.Models
{
    /// <summary>
    /// Prefix format for cloud message topics
    /// </summary>
    public static class TopicPrefix
    {
        //using "-" instead of ":" because firebase does not allow to using : in topic name even when we encode it
        public const string UserTopicPrefix = "userId-{0}";
        public const string AppTopicPrefix = "appId-{0}";
        public const string CourseTopicPrefix = "courseId-{0}";
        public const string RoleTopicPrefix = "roleId-{0}";
    }
}