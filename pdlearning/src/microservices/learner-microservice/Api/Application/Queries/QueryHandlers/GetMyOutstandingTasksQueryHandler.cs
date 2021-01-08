using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyOutstandingTasksQueryHandler : BaseQueryHandler<GetMyOutstandingTasksQuery, PagedResultDto<OutstandingTaskModel>>
    {
        private readonly IRepository<MyOutstandingTask> _myOutstandingTaskRepository;
        private readonly IRepository<MyCourse> _myCourseRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<MyDigitalContent> _myDigitalContentRepository;
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<MyAssignment> _myAssignmentRepository;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IRepository<MyClassRun> _myClassRunRepository;
        private readonly IRepository<ClassRun> _classRunRepository;
        private readonly IRepository<Form> _formRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public GetMyOutstandingTasksQueryHandler(
            IRepository<MyOutstandingTask> myOutstandingTaskRepository,
            IRepository<MyCourse> myCourseRepository,
            IRepository<Course> courseRepository,
            IRepository<MyDigitalContent> myDigitalContentRepository,
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<MyAssignment> myAssignmentRepository,
            IRepository<Assignment> assignmentRepository,
            IRepository<MyClassRun> myClassRunRepository,
            IRepository<ClassRun> classRunRepository,
            IRepository<Form> formRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IUserContext userContext) : base(userContext)
        {
            _myOutstandingTaskRepository = myOutstandingTaskRepository;
            _myCourseRepository = myCourseRepository;
            _courseRepository = courseRepository;
            _myDigitalContentRepository = myDigitalContentRepository;
            _digitalContentRepository = digitalContentRepository;
            _myAssignmentRepository = myAssignmentRepository;
            _assignmentRepository = assignmentRepository;
            _myClassRunRepository = myClassRunRepository;
            _classRunRepository = classRunRepository;
            _formRepository = formRepository;
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task<PagedResultDto<OutstandingTaskModel>> HandleAsync(
            GetMyOutstandingTasksQuery query, CancellationToken cancellationToken)
        {
            var myOutstandingTaskQuery = _myOutstandingTaskRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId);

            var totalCount = await myOutstandingTaskQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResultDto<OutstandingTaskModel>(totalCount);
            }

            var sortCriteria = $"{nameof(MyOutstandingTask.Priority)} ASC, {nameof(MyOutstandingTask.DueDate)} ASC";

            myOutstandingTaskQuery = ApplySorting(myOutstandingTaskQuery, query.PageInfo, sortCriteria);
            myOutstandingTaskQuery = ApplyPaging(myOutstandingTaskQuery, query.PageInfo);

            var myOutstandingTasks = await myOutstandingTaskQuery
                .Select(p => new OutstandingTaskSelection
                {
                    Id = p.Id,
                    ItemId = p.ItemId,
                    ItemType = p.ItemType
                })
                .ToListAsync(cancellationToken);

            var courseTasks = await GetMyOutstandingCourseTask(myOutstandingTasks);
            var assignmentTasks = await GetMyOutstandingAssignmentTask(myOutstandingTasks);
            var microLearningTasks = await GetMyOutstandingMicroLearningTask(myOutstandingTasks);
            var standAloneFormTasks = await GetMyOutstandingStandaloneFormTask(myOutstandingTasks);
            var digitalContentTasks = await GetMyOutstandingDigitalContentTask(myOutstandingTasks);

            var outstandingTasks = courseTasks
                .Concat(assignmentTasks)
                .OrderBy(p => p.DueDate)
                .Concat(standAloneFormTasks)
                .Concat(microLearningTasks)
                .Concat(digitalContentTasks)
                .ToList();

            return new PagedResultDto<OutstandingTaskModel>(totalCount, outstandingTasks);
        }

        /// <summary>
        /// Gets <see cref="OutstandingTaskModel"/> information is based on the user's course information.
        /// </summary>
        /// <param name="myOutstandingTasks">Results from MyOutstandingTask query.</param>
        /// <returns>Returns the list of <see cref="OutstandingTaskModel"/> information.</returns>
        private async Task<IEnumerable<OutstandingTaskModel>> GetMyOutstandingCourseTask(
            List<OutstandingTaskSelection> myOutstandingTasks)
        {
            var registrationIds = FilterTaskByType(myOutstandingTasks, OutstandingTaskType.Course);
            if (!registrationIds.Any())
            {
                return new List<OutstandingTaskModel>();
            }

            // 1. Select Id, StartDateTime, EndDateTime from ClassRun table.
            var classRunQuery = _classRunRepository
                .GetAll()
                .Select(p => new
                {
                    p.Id,
                    p.StartDateTime,
                    p.EndDateTime
                });

            // 2. Select Id and CourseName from ClassRun table.
            var courseQuery = _courseRepository
                .GetAll()
                .Select(p => new
                {
                    p.Id,
                    p.CourseName
                });

            // 3. Select CourseId, ClassRunId, LearningStatus and LearningContentProgress
            // from MyClassRun table then join with (1).
            var myClassRunQuery = _myClassRunRepository
                .GetAll()
                .Where(p => registrationIds.Contains(p.RegistrationId))
                .Select(p => new
                {
                    p.CourseId,
                    p.ClassRunId,
                    p.RegistrationId,
                    p.LearningStatus,
                    p.LearningContentProgress
                })
                .Join(
                    classRunQuery,
                    mcl => mcl.ClassRunId,
                    cl => cl.Id,
                    (mcl, cl) => new
                    {
                        mcl.CourseId,
                        mcl.ClassRunId,
                        mcl.RegistrationId,
                        mcl.LearningStatus,
                        mcl.LearningContentProgress,
                        cl.StartDateTime,
                        cl.EndDateTime
                    });

            // 4. Join (2) and (3) to get the user's course information.
            var myCourseInfoList = await myClassRunQuery
                .Join(
                    courseQuery,
                    mcl => mcl.CourseId,
                    cl => cl.Id,
                    (mcl, c) => new
                    {
                        c.CourseName,
                        mcl.CourseId,
                        mcl.ClassRunId,
                        mcl.RegistrationId,
                        mcl.LearningStatus,
                        mcl.LearningContentProgress,
                        mcl.StartDateTime,
                        mcl.EndDateTime
                    })
                .ToListAsync();

            // 5. To map from user's course information to OutstandingTaskModel.
            return myCourseInfoList.Select(p =>
            {
                var status = p.LearningStatus == LearningStatus.NotStarted
                    ? OutstandingTaskStatus.NotStarted
                    : OutstandingTaskStatus.Continue;

                var progress = p.LearningContentProgress == null
                    ? 0
                    : (float)p.LearningContentProgress;

                var id = GetTaskId(myOutstandingTasks, p.RegistrationId, OutstandingTaskType.Course);

                return OutstandingTaskModel
                    .New(p.CourseName)
                    .WithId(id)
                    .WithStatus(status)
                    .WithStartDate(p.StartDateTime)
                    .WithEndDate(p.EndDateTime)
                    .WithCourseId(p.CourseId)
                    .WithTaskProgress(progress)
                    .WithTaskType(OutstandingTaskType.Course);
            });
        }

        /// <summary>
        /// Gets <see cref="OutstandingTaskModel"/> information is based on the user's participant assignment information.
        /// </summary>
        /// <param name="myOutstandingTasks">Results from MyOutstandingTask query.</param>
        /// <returns>Returns the list of <see cref="OutstandingTaskModel"/> information.</returns>
        private async Task<IEnumerable<OutstandingTaskModel>> GetMyOutstandingAssignmentTask(
            List<OutstandingTaskSelection> myOutstandingTasks)
        {
            var myAssignmentIds = FilterTaskByType(myOutstandingTasks, OutstandingTaskType.Assignment);
            if (!myAssignmentIds.Any())
            {
                return new List<OutstandingTaskModel>();
            }

            // 1. Select Id, CourseId, Title from Assignments table.
            var assignmentQuery = _assignmentRepository
                .GetAll()
                .Select(p => new
                {
                    p.Id,
                    p.CourseId,
                    p.Title
                });

            // 2. Select Status, StartDate, EndDate, AssignmentId from MyAssignments table
            // then join with (1) to get the user's participant assignment information.
            var myAssignmentInfoList = await _myAssignmentRepository
                .GetAll()
                .Where(p => myAssignmentIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Status,
                    p.StartDate,
                    p.EndDate,
                    p.AssignmentId
                })
                .Join(
                    assignmentQuery,
                    ma => ma.AssignmentId,
                    a => a.Id,
                    (ma, a) => new
                    {
                        ma.Id,
                        a.CourseId,
                        a.Title,
                        ma.Status,
                        ma.StartDate,
                        ma.EndDate,
                        ma.AssignmentId
                    })
                .ToListAsync();

            // 3. To map from user's participant assignment information to OutstandingTaskModel.
            return myAssignmentInfoList.Select(p =>
            {
                var status = p.Status == MyAssignmentStatus.NotStarted
                    ? OutstandingTaskStatus.NotStarted
                    : OutstandingTaskStatus.Continue;

                var id = GetTaskId(myOutstandingTasks, p.Id, OutstandingTaskType.Assignment);

                return OutstandingTaskModel
                    .New(p.Title)
                    .WithId(id)
                    .WithStatus(status)
                    .WithStartDate(p.StartDate)
                    .WithEndDate(p.EndDate)
                    .WithCourseId(p.CourseId)
                    .WithAssignmentId(p.AssignmentId)
                    .WithTaskType(OutstandingTaskType.Assignment);
            });
        }

        /// <summary>
        /// Gets <see cref="OutstandingTaskModel"/> information is based on the user's microlearning course information.
        /// </summary>
        /// <param name="myOutstandingTasks">Results from MyOutstandingTask query.</param>
        /// <returns>Returns the list of <see cref="OutstandingTaskModel"/> information.</returns>
        private async Task<IEnumerable<OutstandingTaskModel>> GetMyOutstandingMicroLearningTask(
            List<OutstandingTaskSelection> myOutstandingTasks)
        {
            var myCourseIds = FilterTaskByType(myOutstandingTasks, OutstandingTaskType.Microlearning);
            if (!myCourseIds.Any())
            {
                return new List<OutstandingTaskModel>();
            }

            // 1. Select Id, CourseName from Courses table.
            var courseQuery = _courseRepository
                .GetAll()
                .Select(c => new
                {
                    c.Id,
                    c.CourseName
                });

            // 2. Select Status, CourseId, ProgressMeasure from MyCourses table
            // then join with (1) to get the user's microlearning course information.
            var myCourseInfoList = await _myCourseRepository
                .GetAll()
                .Where(p => myCourseIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Status,
                    p.CourseId,
                    p.ProgressMeasure
                })
                .Join(
                    courseQuery,
                    mc => mc.CourseId,
                    c => c.Id,
                    (mc, c) => new
                    {
                        mc.Id,
                        mc.Status,
                        mc.CourseId,
                        mc.ProgressMeasure,
                        c.CourseName
                    })
                .ToListAsync();

            // 3. To map from user's microlearning course information to OutstandingTaskModel.
            return myCourseInfoList.Select(p =>
            {
                var status = p.Status == MyCourseStatus.NotStarted
                    ? OutstandingTaskStatus.NotStarted
                    : OutstandingTaskStatus.Continue;

                var progress = p.ProgressMeasure.HasValue
                    ? (float)p.ProgressMeasure
                    : 0;

                var id = GetTaskId(myOutstandingTasks, p.Id, OutstandingTaskType.Microlearning);

                return OutstandingTaskModel
                    .New(p.CourseName)
                    .WithId(id)
                    .WithStatus(status)
                    .WithCourseId(p.CourseId)
                    .WithTaskProgress(progress)
                    .WithTaskType(OutstandingTaskType.Microlearning);
            });
        }

        /// <summary>
        /// Gets <see cref="OutstandingTaskModel"/> information is based on the user's digital content information.
        /// </summary>
        /// <param name="myOutstandingTasks">Results from MyOutstandingTask query.</param>
        /// <returns>Returns the list of <see cref="OutstandingTaskModel"/> information.</returns>
        private async Task<IEnumerable<OutstandingTaskModel>> GetMyOutstandingDigitalContentTask(
            List<OutstandingTaskSelection> myOutstandingTasks)
        {
            var myDigitalContentIds = FilterTaskByType(myOutstandingTasks, OutstandingTaskType.DigitalContent);
            if (!myDigitalContentIds.Any())
            {
                return new List<OutstandingTaskModel>();
            }

            // 1. Select Id, Title, FileExtension from DigitalContents table.
            var digitalContentQuery = _digitalContentRepository
                .GetAll()
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.FileExtension
                });

            // 2. Select Status, DigitalContentId, ProgressMeasure from MyDigitalContent table
            // then join with (1) to get the user's digital content information.
            var myDigitalContentInfoList = await _myDigitalContentRepository
                .GetAll()
                .Where(p => myDigitalContentIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Status,
                    p.DigitalContentId,
                    p.ProgressMeasure
                })
                .Join(
                    digitalContentQuery,
                    mdc => mdc.DigitalContentId,
                    dc => dc.Id,
                    (mdc, dc) => new
                    {
                        mdc.Id,
                        dc.Title,
                        dc.FileExtension,
                        mdc.ProgressMeasure,
                        mdc.DigitalContentId,
                        mdc.Status
                    })
                .ToListAsync();

            // 3. To map from the user's digital content information to OutstandingTaskModel.
            return myDigitalContentInfoList.Select(p =>
            {
                var status = p.Status == MyDigitalContentStatus.NotStarted
                    ? OutstandingTaskStatus.NotStarted
                    : OutstandingTaskStatus.Continue;

                var fileExtension = p.FileExtension != null
                    ? FileExtensionMapper.MapFromFileExtension(p.FileExtension)
                    : null;

                var id = GetTaskId(myOutstandingTasks, p.Id, OutstandingTaskType.DigitalContent);

                return OutstandingTaskModel
                    .New(p.Title)
                    .WithId(id)
                    .WithStatus(status)
                    .WithFileExtension(fileExtension)
                    .WithDigitalContentId(p.DigitalContentId)
                    .WithTaskType(OutstandingTaskType.DigitalContent);
            });
        }

        /// <summary>
        /// Gets <see cref="OutstandingTaskModel"/> information is based on the user's form participant information.
        /// </summary>
        /// <param name="myOutstandingTasks">Results from MyOutstandingTask query.</param>
        /// <returns>Returns the list of <see cref="OutstandingTaskModel"/> information.</returns>
        private async Task<IEnumerable<OutstandingTaskModel>> GetMyOutstandingStandaloneFormTask(
            List<OutstandingTaskSelection> myOutstandingTasks)
        {
            var formParticipantIds = FilterTaskByType(myOutstandingTasks, OutstandingTaskType.StandaloneForm);
            if (!formParticipantIds.Any())
            {
                return new List<OutstandingTaskModel>();
            }

            // 1. Select OriginalObjectId, Title, StartDate, EndDate from Forms table.
            var formQuery = _formRepository
                .GetAll()
                .Where(p => p.Status == FormStatus.Published)
                .Where(p => p.IsStandalone.Value)
                .Where(p => !p.IsArchived)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new
                {
                    p.OriginalObjectId,
                    p.Title,
                    p.StartDate,
                    p.EndDate,
                    p.Type
                });

            // 2. Select Status, FormOriginalObjectId from FormParticipants table
            // then join with (1) to get the user's form participant information.
            var formParticipants = await _formParticipantRepository
                .GetAll()
                .Where(p => formParticipantIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Status,
                    p.FormOriginalObjectId
                })
                .Join(
                    formQuery,
                    fp => fp.FormOriginalObjectId,
                    f => f.OriginalObjectId,
                    (fp, f) => new
                    {
                        fp.Id,
                        f.Title,
                        f.StartDate,
                        f.EndDate,
                        fp.Status,
                        fp.FormOriginalObjectId,
                        f.Type
                    })
                .ToListAsync();

            // 3. To map from the user's digital content information to OutstandingTaskModel.
            return formParticipants.Select(p =>
            {
                var status = p.Status == FormParticipantStatus.NotStarted
                    ? OutstandingTaskStatus.NotStarted
                    : OutstandingTaskStatus.Continue;

                var id = GetTaskId(myOutstandingTasks, p.Id, OutstandingTaskType.StandaloneForm);

                return OutstandingTaskModel
                    .New(p.Title)
                    .WithId(id)
                    .WithStatus(status)
                    .WithFormId(p.FormOriginalObjectId)
                    .WithStartDate(p.StartDate)
                    .WithEndDate(p.EndDate)
                    .WithFormType(p.Type)
                    .WithTaskType(OutstandingTaskType.StandaloneForm);
            });
        }

        private Guid? GetTaskId(
            List<OutstandingTaskSelection> outstandingTasks,
            Guid itemId,
            OutstandingTaskType type)
        {
            return outstandingTasks
                .Where(p => p.ItemType == type && p.ItemId == itemId)
                .Select(p => p.Id)
                .FirstOrDefault();
        }

        private List<Guid> FilterTaskByType(
            List<OutstandingTaskSelection> outstandingTasks,
            OutstandingTaskType type)
        {
            return outstandingTasks
                .Where(p => p.ItemType == type)
                .Select(p => p.ItemId)
                .ToList();
        }
    }

    internal class OutstandingTaskSelection
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }

        public OutstandingTaskType ItemType { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
