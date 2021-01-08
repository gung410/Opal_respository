namespace Microservice.Badge.Domain.Enums
{
    /// <summary>
    /// Enum define activity type of user action.
    /// </summary>
    public enum ActivityType
    {
        Unknown,

        /// <summary>
        /// Comment to others post.
        /// </summary>
        CommentOthersPost,

        /// <summary>
        /// Comment to self post.
        /// </summary>
        CommentSelfPost,
        CommentForumPost,
        CreateForum,
        PostCommunity,
        PostUserWall,
        Forward,
        LikePost,
        FollowCommunity,

        /// <summary>
        /// Experience reflection.
        /// </summary>
        CreateReflection,

        /// <summary>
        /// Showcase shared reflection.
        /// </summary>
        CreateSharedReflection,

        CompleteMLU,
        CompleteDigitalResources,
        CompleteElearning,
        CompleteAnotherCourse,

        /// <summary>
        /// User created a MLU course.
        /// </summary>
        CreatedMLU,

        /// <summary>
        /// User created a learning path.
        /// </summary>
        CreatedLearningPath,

        /// <summary>
        /// User shared a learning path.
        /// </summary>
        SharedLearningPath,

        /// <summary>
        /// User bookmarked a learning path.
        /// </summary>
        BookmarkedLearningPath
    }
}
