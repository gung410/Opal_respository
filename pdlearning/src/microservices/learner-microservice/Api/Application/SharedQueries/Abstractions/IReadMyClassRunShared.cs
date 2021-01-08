using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.SharedQueries.Abstractions
{
    public interface IReadMyClassRunShared : ISharedQuery
    {
        /// <summary>
        /// Ensure a new my class run has been inserted into database.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseId">The identifier course.</param>
        /// <returns>Returns true if a new my class run has been inserted into database, otherwise false.</returns>
        Task<bool> EnsureMyClassRunInserted(Guid userId, Guid courseId);

        /// <summary>
        /// To get <see cref="MyClassRun"/> is in-progress.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseId">The identifier course.</param>
        /// <returns>Returns <see cref="MyClassRun"/> is in-progress on workflow by UserId and CourseId.
        /// </returns>
        Task<List<MyClassRun>> GetInProgressRegistrations(Guid userId, Guid courseId);

        /// <summary>
        /// To get the list of <see cref="MyClassRun"/> in-progress by the identifier class run.
        /// </summary>
        /// <param name="classRunId">The identifier class run.</param>
        /// <param name="allRegistrations">Flag to get all registrations of class run.</param>
        /// <returns>Returns the list of <see cref="MyClassRun"/> in-progress on the workflow
        /// by ClassRunId and CourseId.</returns>
        Task<List<MyClassRun>> GetInProgressRegistrationsByClassRunId(Guid classRunId, bool allRegistrations = false);

        /// <summary>
        /// To get the list of <see cref="MyClassRunBasicInfoModel"/> information is not participants.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseIds">The list of identifier course.</param>
        /// <returns>Returns the list <see cref="MyClassRunBasicInfoModel"/> rejected
        /// or withdrawn on the workflow by UserId and CourseIds.</returns>
        Task<List<MyClassRunBasicInfoModel>> GetNotParticipants(Guid userId, List<Guid> courseIds);

        /// <summary>
        /// To get <see cref="MyClassRun"/> expired.
        /// </summary>
        /// <param name="registrationIds">The list of identifier registration.</param>
        /// <returns>A dictionary that with Key is course,
        /// and Value are ClassRunId and <see cref="Domain.ValueObject.RegistrationType"/>
        /// of each registration.</returns>
        Task<Dictionary<Guid, MyClassRunBasicInfoModel>> GetExpiredRegistrations(List<Guid> registrationIds);

        /// <summary>
        /// To get the list <see cref="MyClassRun"/> are in-progress and completed.
        /// </summary>
        /// <param name="userId">The identifier user.</param>
        /// <param name="courseIds">The identifier course list.</param>
        /// <returns>Returns the list of <see cref="MyClassRun"/> in-progress and completed on workflow
        /// by UserId and CourseIds.</returns>
        Task<List<MyClassRun>> GetNotExpiredRegistrations(Guid userId, List<Guid> courseIds);
    }
}
