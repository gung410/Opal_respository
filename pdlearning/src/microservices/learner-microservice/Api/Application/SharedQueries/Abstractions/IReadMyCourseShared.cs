using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.SharedQueries.Abstractions
{
    public interface IReadMyCourseShared : ISharedQuery
    {
        /// <summary>
        /// Get completed times on the current course.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseIds">The identifier course.</param>
        /// <returns>Returns the number of completions in the current course.</returns>
        Task<int> GetCompletedTimes(Guid userId, List<Guid> courseIds);

        /// <summary>
        /// To get the latest user's course information by current UserId and CourseId.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseId">The identifier course.</param>
        /// <returns>The latest user's course information <see cref="MyCourse"/>.</returns>
        Task<MyCourse> GetByUserIdAndCourseId(Guid userId, Guid courseId);

        /// <summary>
        /// Build the query to get the my course information by CourseId
        /// and list course id.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseIds">List course id.</param>
        /// <returns>The user's course information <see cref="MyCourse"/> by RegistrationId.</returns>
        Task<List<MyCourse>> GetByUserIdAndCourseIds(Guid userId, List<Guid> courseIds);

        /// <summary>
        /// Creates query by CurrentUserId and <see cref="MyCourseStatus"/> list.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="statuses">My course status list.</param>
        /// <returns>An <see cref="IQueryable"/> that contains the <see cref="MyCourse"/> information.</returns>
        IQueryable<MyCourse> FilterByUserIdAndStatusQuery(Guid userId, List<MyCourseStatus> statuses);

        /// <summary>
        /// Creates query by CurrentUserId and <see cref="CourseType"/>.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseType">CourseType as value object.</param>
        /// <returns>An <see cref="IQueryable"/> that contains the <see cref="MyCourse"/> information.</returns>
        IQueryable<MyCourse> FilterByUserIdAndCourseTypeQuery(Guid userId, LearningCourseType courseType);

        /// <summary>
        /// Creates query by CurrentUserId and list of the registration identifier.
        /// </summary>
        /// <param name="registrationIds">List of the identifier registration.</param>
        /// <returns>An <see cref="IQueryable"/> that contains the <see cref="MyCourse"/> information.</returns>
        IQueryable<MyCourse> FilterByRegistrationIdsQuery(List<Guid> registrationIds);

        /// <summary>
        /// Creates query by CurrentUserId and <see cref="MyCourseStatus"/> list.
        /// </summary>
        /// <param name="courseId">The identifier course.</param>
        /// <param name="courseType">CourseType as value object.</param>
        /// <returns>An <see cref="IQueryable"/> that contains the <see cref="MyCourse"/> information.</returns>
        IQueryable<MyCourse> FilterByCourseIdAndCourseTypeQuery(Guid courseId, LearningCourseType courseType);

        /// <summary>
        /// To get related info of courses such as:
        /// Rating, review, class run, completed times.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseIds">List Id of courses.</param>
        /// <param name="userBookmarks">List user bookmark entities.</param>
        /// <param name="myCourses">List my courses entities.</param>
        /// <returns>List of courses with related info.</returns>
        Task<List<CourseModel>> GetRelatedInfoOfCourses(Guid userId, List<Guid> courseIds, List<UserBookmark> userBookmarks, List<MyCourse> myCourses);
    }
}
