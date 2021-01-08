using System;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.BusinessLogic.Abstractions
{
    public interface IWriteMyOutstandingTaskLogic : IBusinessLogic
    {
        /// <summary>
        /// Insert micro-learning course task with <see cref="MyOutstandingTask.ItemId"/> is stored as the identifier <see cref="MyCourse"/>.
        /// </summary>
        /// <param name="myCourse">The identifier <see cref="MyCourse"/>.</param>
        /// <returns>No results are returned.</returns>
        Task InsertMicroLearningTask(MyCourse myCourse);

        /// <summary>
        /// Insert course task with <see cref="MyOutstandingTask.ItemId"/> is stored as the identifier registration.
        /// </summary>
        /// <param name="myClassRun">MyClassRun entity.</param>
        /// <returns>No results are returned.</returns>
        Task InsertCourseTask(MyClassRun myClassRun);

        /// <summary>
        /// Insert assignment task with <see cref="MyOutstandingTask.ItemId"/> is stored as the identifier <see cref="MyAssignment"/>.
        /// </summary>
        /// <param name="myAssignment">MyAssignment entity.</param>
        /// <returns>No results are returned.</returns>
        Task InsertAssignmentTask(MyAssignment myAssignment);

        /// <summary>
        /// Insert digital content task with <see cref="MyOutstandingTask.ItemId"/> is stored as the identifier <see cref="MyDigitalContent"/>.
        /// </summary>
        /// <param name="myDigitalContent">MyDigitalContent entity.</param>
        /// <returns>No results are returned.</returns>
        Task InsertDigitalContentTask(MyDigitalContent myDigitalContent);

        /// <summary>
        /// Insert many standalone form task.
        /// </summary>
        /// <param name="formOriginalObjectId">The original id of the form.</param>
        /// <returns>No results are returned.</returns>
        Task InsertManyStandaloneFormTask(Guid formOriginalObjectId);

        /// <summary>
        /// Delete course task by UserId and the identifier registration
        /// and the type of task is <see cref="OutstandingTaskType.Course"/>.
        /// </summary>
        /// <param name="myClassRun">MyClassRun entity.</param>
        /// <returns>No results are returned.</returns>
        Task DeleteCourseTask(MyClassRun myClassRun);

        /// <summary>
        /// Delete micro-learning course task by UserId, the identifier <see cref="MyCourse"/>
        /// and the type of task is <see cref="OutstandingTaskType.Microlearning"/>.
        /// </summary>
        /// <param name="myCourse">MyCourse entity.</param>
        /// <returns>No results are returned.</returns>
        Task DeleteMicroLearningTask(MyCourse myCourse);

        /// <summary>
        /// Delete digital content task by UserId, the identifier <see cref="MyDigitalContent"/>
        /// and the type of task is <see cref="OutstandingTaskType.DigitalContent"/>.
        /// </summary>
        /// <param name="myDigitalContent">MyDigitalContent entity.</param>
        /// <returns>No results are returned.</returns>
        Task DeleteDigitalContentTask(MyDigitalContent myDigitalContent);

        /// <summary>
        /// Delete assignment course task by UserId, the identifier <see cref="MyAssignment"/>
        /// and the type of task is <see cref="OutstandingTaskType.Assignment"/>.
        /// </summary>
        /// <param name="myAssignment">MyAssignment entity.</param>
        /// <returns>No results are returned.</returns>
        Task DeleteAssignmentTask(MyAssignment myAssignment);

        /// <summary>
        /// Delete standalone form course task by UserId, the identifier <see cref="FormParticipant"/>
        /// and the type of task is <see cref="OutstandingTaskType.StandaloneForm"/>.
        /// </summary>
        /// <param name="formParticipant">FormParticipant entity.</param>
        /// <returns>No results are returned.</returns>
        Task DeleteStandaloneFormTask(FormParticipant formParticipant);

        /// <summary>
        /// Delete many standalone form task.
        /// </summary>
        /// <param name="formOriginalObjectId">The original id of the form.</param>
        /// <returns>No results are returned.</returns>
        Task DeleteManyStandaloneFormTask(Guid formOriginalObjectId);

        /// <summary>
        /// Delete many course task.
        /// </summary>
        /// <param name="classRun">ClassRun entity.</param>
        /// <returns>No results are returned.</returns>
        Task DeleteManyCourseTask(ClassRun classRun);

        /// <summary>
        /// Update due date of course task.
        /// </summary>
        /// <param name="classRun">ClassRun entity.</param>
        /// <param name="endDateTime">End date of class run.</param>
        /// <returns>No results are returned.</returns>
        Task UpdateDueDateOfCourseTask(ClassRun classRun, DateTime? endDateTime);
    }
}
