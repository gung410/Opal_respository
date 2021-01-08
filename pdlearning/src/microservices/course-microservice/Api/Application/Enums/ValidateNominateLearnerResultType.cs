namespace Microservice.Course.Application.Enums
{
    public enum ValidateLearnerResultType
    {
        /// <summary>
        /// ByAssignments result valid.
        /// </summary>
        Valid,

        /// <summary>
        /// Learner reached max re-learning time.
        /// </summary>
        MaxReLearningTimes,

        /// <summary>
        /// Learner has incomplete registration added by CAM.
        /// </summary>
        HasUncompleteRegistrationAddedByCA,

        /// <summary>
        /// Learner has incomplete registration added by Learner.
        /// </summary>
        HasUncompleteRegistrationAddedByLearner,

        /// <summary>
        /// Learner violate Requisite Courses.
        /// </summary>
        HasPreRequisiteCoursesViolated
    }
}
