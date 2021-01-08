using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class EnrollCourseCommandHandler : BaseCommandHandler<EnrollCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IWriteMyCourseLogic _writeMyCourseLogic;
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IRepository<ClassRun> _classRunRepository;
        private readonly IRepository<MyClassRun> _myClassRunRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCud;
        private readonly IRepository<LectureInMyCourse> _lectureInMyCourseRepository;

        public EnrollCourseCommandHandler(
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            IWriteMyCourseLogic writeMyCourseLogic,
            IRepository<Course> courseRepository,
            IReadMyCourseShared readMyCourseShared,
            IRepository<ClassRun> classRunRepository,
            IRepository<MyClassRun> myClassRunRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCud,
            IRepository<LectureInMyCourse> lectureInMyCourseRepository) : base(unitOfWorkManager, userContext)
        {
            _courseRepository = courseRepository;
            _readMyCourseShared = readMyCourseShared;
            _writeMyCourseLogic = writeMyCourseLogic;
            _classRunRepository = classRunRepository;
            _myClassRunRepository = myClassRunRepository;
            _myOutstandingTaskCud = myOutstandingTaskCud;
            _lectureInMyCourseRepository = lectureInMyCourseRepository;
        }

        protected override async Task HandleAsync(EnrollCourseCommand command, CancellationToken cancellationToken)
        {
            var existingMyCourse = await _readMyCourseShared
                .GetByUserIdAndCourseId(CurrentUserIdOrDefault, command.CourseId);

            var course = await _courseRepository
                .GetAll()
                .Where(p => p.Id == command.CourseId)
                .FirstOrDefaultAsync(cancellationToken);

            if (course == null)
            {
                throw new EntityNotFoundException();
            }

            if (existingMyCourse != null && (existingMyCourse.IsMicroLearning() || existingMyCourse.IsStarted()))
            {
                throw new EnrolledCourseException();
            }

            // For the course is MicroLearning.
            if (existingMyCourse == null && course.IsMicroLearning())
            {
                // validate lectures because in the payload we don't check lectures content when user start learning
                if (command.LectureIds == null || !command.LectureIds.Any())
                {
                    throw new InvalidLecturesException();
                }

                await EnrollWithMicroLearning(command);
            }

            // For the course is non MicroLearning
            else if (existingMyCourse != null && !course.IsMicroLearning())
            {
                await EnrollWithNonMicroLearning(existingMyCourse, command);
            }
        }

        private async Task EnrollWithMicroLearning(EnrollCourseCommand command)
        {
            var enrollDate = Clock.Now;

            var myCourse = new MyCourse
            {
                Id = command.Id,
                CourseId = command.CourseId,
                UserId = CurrentUserIdOrDefault,
                CreatedBy = CurrentUserIdOrDefault,
                Status = MyCourseStatus.InProgress,
                ProgressMeasure = 0,
                StartDate = enrollDate,
                LastLogin = enrollDate,
                CreatedDate = enrollDate
            };

            await AddLectures(myCourse, command);

            await _writeMyCourseLogic.Insert(myCourse);

            await _myOutstandingTaskCud.InsertMicroLearningTask(myCourse);
        }

        private async Task EnrollWithNonMicroLearning(MyCourse existingMyCourse, EnrollCourseCommand command)
        {
            var enrollDate = Clock.Now;

            var validLearningTime = await ValidLearningTime(enrollDate, existingMyCourse);
            if (!validLearningTime)
            {
                throw new InValidLearningException();
            }

            existingMyCourse.Status = MyCourseStatus.InProgress;
            existingMyCourse.StartDate = enrollDate;

            if (command.LectureIds != null && command.LectureIds.Any())
            {
                await AddLectures(existingMyCourse, command);
            }
            else
            {
                // By default: If not microlearning && no lecture in course => we must set 100% learning progress
                existingMyCourse.ProgressMeasure = 100;
            }

            await _writeMyCourseLogic.Update(existingMyCourse);
        }

        private async Task AddLectures(MyCourse myCourse, EnrollCourseCommand command)
        {
            var existedLectures = await _lectureInMyCourseRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.MyCourseId == myCourse.Id)
                .AnyAsync();

            if (existedLectures)
            {
                throw new LectureDuplicatedException();
            }

            if (command.LectureIds.Any())
            {
                var lectures = command.LectureIds
                    .Select(lecture => new LectureInMyCourse
                    {
                        Id = Guid.NewGuid(),
                        MyCourseId = myCourse.Id,
                        LectureId = lecture,
                        UserId = CurrentUserIdOrDefault,
                        CreatedBy = CurrentUserIdOrDefault,
                        Status = LectureStatus.NotStarted,
                        StartDate = myCourse.StartDate
                    });

                await _lectureInMyCourseRepository.InsertManyAsync(lectures);
            }
        }

        /// <summary>
        /// Validate learn time for course is non micro-learning.
        /// </summary>
        /// <param name="enrollDate">Enroll date.</param>
        /// <param name="myCourse">MyCourse entity.</param>
        /// <returns>Returns true if the class run is started and not ended otherwise false.</returns>
        private Task<bool> ValidLearningTime(DateTime enrollDate, MyCourse myCourse)
        {
            var myClassRunQuery = _myClassRunRepository
                .GetAll()
                .Where(p => p.RegistrationId == myCourse.RegistrationId)
                .Where(MyClassRun.FilterParticipantExpr());

            var classRunQuery = _classRunRepository
                .GetAll()
                .Where(p => p.StartDateTime.Value.Date <= enrollDate)
                .Where(p => p.EndDateTime.Value.Date.AddDays(1) >= enrollDate.Date);

            return myClassRunQuery
                .Join(
                    classRunQuery,
                    mcl => mcl.ClassRunId,
                    cl => cl.Id,
                    (myClassRun, classRun) => myClassRun)
                .AnyAsync();
        }
    }
}
