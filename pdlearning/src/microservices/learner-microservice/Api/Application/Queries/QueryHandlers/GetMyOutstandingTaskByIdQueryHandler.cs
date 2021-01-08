using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyOutstandingTaskByIdQueryHandler : BaseQueryHandler<GetMyOutstandingTaskByIdQuery, OutstandingTaskModel>
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

        public GetMyOutstandingTaskByIdQueryHandler(
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

        protected override async Task<OutstandingTaskModel> HandleAsync(GetMyOutstandingTaskByIdQuery query, CancellationToken cancellationToken)
        {
            var task = await _myOutstandingTaskRepository
                .GetAll()
                .Where(p => p.Id == query.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (task == null)
            {
                // Outstanding task completed.
                return null;
            }

            switch (task.ItemType)
            {
                case OutstandingTaskType.Course:
                    return await GetMyOutstandingCourseTask(task);
                case OutstandingTaskType.Assignment:
                    return await GetMyOutstandingAssignmentTask(task);
                case OutstandingTaskType.DigitalContent:
                    return await GetMyOutstandingDigitalContentTask(task);
                case OutstandingTaskType.StandaloneForm:
                    return await GetMyOutstandingStandaloneFormTask(task);
                case OutstandingTaskType.Microlearning:
                    return await GetMyOutstandingMicroLearningTask(task);
                default:
                    throw new NotSupportedException();
            }
        }

        private async Task<OutstandingTaskModel> GetMyOutstandingCourseTask(MyOutstandingTask task)
        {
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
                .Where(p => p.RegistrationId == task.ItemId)
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
            var myCourseInfo = await myClassRunQuery
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
                .FirstOrDefaultAsync();

            // 5. To map from user's course information to OutstandingTaskModel.
            var status = myCourseInfo.LearningStatus == LearningStatus.NotStarted
                ? OutstandingTaskStatus.NotStarted
                : OutstandingTaskStatus.Continue;

            var progress = myCourseInfo.LearningContentProgress == null
                ? 0
                : (float)myCourseInfo.LearningContentProgress;

            return OutstandingTaskModel
                .New(myCourseInfo.CourseName)
                .WithId(task.Id)
                .WithStatus(status)
                .WithStartDate(myCourseInfo.StartDateTime)
                .WithEndDate(myCourseInfo.EndDateTime)
                .WithCourseId(myCourseInfo.CourseId)
                .WithTaskProgress(progress)
                .WithTaskType(OutstandingTaskType.Course);
        }

        private async Task<OutstandingTaskModel> GetMyOutstandingAssignmentTask(MyOutstandingTask task)
        {
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
            var myAssignmentInfo = await _myAssignmentRepository
                .GetAll()
                .Where(p => p.Id == task.ItemId)
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
                .FirstOrDefaultAsync();

            // 3. To map from user's participant assignment information to OutstandingTaskModel.
            var status = myAssignmentInfo.Status == MyAssignmentStatus.NotStarted
                ? OutstandingTaskStatus.NotStarted
                : OutstandingTaskStatus.Continue;

            return OutstandingTaskModel
                .New(myAssignmentInfo.Title)
                .WithId(task.Id)
                .WithStatus(status)
                .WithStartDate(myAssignmentInfo.StartDate)
                .WithEndDate(myAssignmentInfo.EndDate)
                .WithCourseId(myAssignmentInfo.CourseId)
                .WithAssignmentId(myAssignmentInfo.AssignmentId)
                .WithTaskType(OutstandingTaskType.Assignment);
        }

        private async Task<OutstandingTaskModel> GetMyOutstandingMicroLearningTask(MyOutstandingTask task)
        {
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
            var myCourseInfo = await _myCourseRepository
                .GetAll()
                .Where(p => p.Id == task.ItemId)
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
                .FirstOrDefaultAsync();

            // 3. To map from user's microlearning course information to OutstandingTaskModel.
            var status = myCourseInfo.Status == MyCourseStatus.NotStarted
                ? OutstandingTaskStatus.NotStarted
                : OutstandingTaskStatus.Continue;

            var progress = myCourseInfo.ProgressMeasure.HasValue
                ? (float)myCourseInfo.ProgressMeasure
                : 0;

            return OutstandingTaskModel
                .New(myCourseInfo.CourseName)
                .WithId(task.Id)
                .WithStatus(status)
                .WithCourseId(myCourseInfo.CourseId)
                .WithTaskProgress(progress)
                .WithTaskType(OutstandingTaskType.Microlearning);
        }

        private async Task<OutstandingTaskModel> GetMyOutstandingDigitalContentTask(MyOutstandingTask task)
        {
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
            var myDigitalContentInfo = await _myDigitalContentRepository
                .GetAll()
                .Where(p => p.Id == task.ItemId)
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
                .FirstOrDefaultAsync();

            // 3. To map from the user's digital content information to OutstandingTaskModel.
            var status = myDigitalContentInfo.Status == MyDigitalContentStatus.NotStarted
                ? OutstandingTaskStatus.NotStarted
                : OutstandingTaskStatus.Continue;

            var fileExtension = myDigitalContentInfo.FileExtension != null
                ? FileExtensionMapper.MapFromFileExtension(myDigitalContentInfo.FileExtension)
                : null;

            return OutstandingTaskModel
                .New(myDigitalContentInfo.Title)
                .WithId(task.Id)
                .WithStatus(status)
                .WithFileExtension(fileExtension)
                .WithDigitalContentId(myDigitalContentInfo.DigitalContentId)
                .WithTaskType(OutstandingTaskType.DigitalContent);
        }

        private async Task<OutstandingTaskModel> GetMyOutstandingStandaloneFormTask(MyOutstandingTask task)
        {
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
            var formParticipantInfo = await _formParticipantRepository
                .GetAll()
                .Where(p => p.Id == task.ItemId)
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
                .FirstOrDefaultAsync();

            // 3. To map from the user's digital content information to OutstandingTaskModel.
            var status = formParticipantInfo.Status == FormParticipantStatus.NotStarted
                ? OutstandingTaskStatus.NotStarted
                : OutstandingTaskStatus.Continue;

            return OutstandingTaskModel
                .New(formParticipantInfo.Title)
                .WithId(formParticipantInfo.Id)
                .WithStatus(status)
                .WithFormId(formParticipantInfo.FormOriginalObjectId)
                .WithStartDate(formParticipantInfo.StartDate)
                .WithEndDate(formParticipantInfo.EndDate)
                .WithFormType(formParticipantInfo.Type)
                .WithTaskType(OutstandingTaskType.StandaloneForm);
        }
    }
}
