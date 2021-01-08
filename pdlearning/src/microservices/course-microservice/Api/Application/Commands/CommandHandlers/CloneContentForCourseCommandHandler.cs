using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CloneContentForCourseCommandHandler : BaseCommandHandler<CloneContentForCourseCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IReadOnlyRepository<QuizAssignmentForm> _readQuizAssignmentFormRepository;
        private readonly IReadOnlyRepository<QuizAssignmentFormQuestion> _readQuizAssignmentFormQuestionRepository;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly LectureCudLogic _lectureCudLogic;
        private readonly SectionCudLogic _sectionCudLogic;
        private readonly AssignmentCudLogic _assignmentCudLogic;

        public CloneContentForCourseCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IReadOnlyRepository<QuizAssignmentForm> readQuizAssignmentFormRepository,
            IReadOnlyRepository<QuizAssignmentFormQuestion> readQuizAssignmentFormQuestionRepository,
            ProcessPostSavingContentLogic processPostSavingContentLogic,
            LectureCudLogic lectureCudLogic,
            SectionCudLogic sectionCudLogic,
            AssignmentCudLogic assignmentCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readSectionRepository = readSectionRepository;
            _readLectureRepository = readLectureRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _readLectureContentRepository = readLectureContentRepository;
            _readQuizAssignmentFormRepository = readQuizAssignmentFormRepository;
            _readQuizAssignmentFormQuestionRepository = readQuizAssignmentFormQuestionRepository;
            _processPostSavingContentLogic = processPostSavingContentLogic;
            _lectureCudLogic = lectureCudLogic;
            _sectionCudLogic = sectionCudLogic;
            _assignmentCudLogic = assignmentCudLogic;
        }

        protected override async Task HandleAsync(CloneContentForCourseCommand command, CancellationToken cancellationToken)
        {
            // Get course
            var fromCourse = await _readCourseRepository.GetAsync(command.FromCourseId);
            var toCourse = await _readCourseRepository.GetAsync(command.ToCourseId);

            EnsureBusinessLogicValid(fromCourse.ValidateNotArchived());

            EnsureValidPermission(
                fromCourse.HasContentCreatorPermission(CurrentUserId, CurrentUserRoles, _readCourseRepository.GetHasAdminRightChecker(fromCourse, AccessControlContext)));

            // Get Course Contents
            var sections = await _readSectionRepository.GetAllListAsync(_ => _.CourseId == command.FromCourseId && _.ClassRunId == null);
            var lectures = await _readLectureRepository.GetAllListAsync(_ => _.CourseId == command.FromCourseId && _.ClassRunId == null);
            var lectureIds = lectures.Select(x => x.Id);
            var lectureContents = await _readLectureContentRepository.GetAllListAsync(_ => lectureIds.Contains(_.LectureId));
            var assignments = await _readAssignmentRepository.GetAllListAsync(_ => _.CourseId == command.FromCourseId && _.ClassRunId == null);
            var assignmentIds = assignments.Select(x => x.Id);
            var assignmentForms = await _readQuizAssignmentFormRepository.GetAllListAsync(_ => assignmentIds.Contains(_.Id));
            var assignmentFormQuestions = await _readQuizAssignmentFormQuestionRepository.GetAllListAsync(_ => assignmentIds.Contains(_.QuizAssignmentFormId));

            // Get course max order number of Course Content
            var toCourseMaxOrderNumber = await GetMaxOrderNumberOfCourseAsync(command.ToCourseId, cancellationToken);

            // Clone contents and update the data to match for Section
            var clonedSectionsMapByOldId = sections.ToDictionary(p => p.Id, p => p.CloneForCourse(command.ToCourseId, CurrentUserIdOrDefault, toCourseMaxOrderNumber));
            var clonedLectureMapByOldId = lectures.ToDictionary(p => p.Id, p =>
            {
                var newSectionId = p.SectionId == null || !clonedSectionsMapByOldId.ContainsKey(p.SectionId.Value) ? null : (Guid?)clonedSectionsMapByOldId[p.SectionId.Value].Id;
                return p.CloneForCourse(command.ToCourseId, CurrentUserIdOrDefault, newSectionId, toCourseMaxOrderNumber);
            });
            var clonedContentLectures = lectureContents.Select(p =>
            {
                var newLectureId = clonedLectureMapByOldId[p.LectureId].Id;
                return p.Clone(CurrentUserIdOrDefault, newLectureId);
            });
            var clonedAssignmentsMapByOldId = assignments.ToDictionary(p => p.Id, p => p.CloneForCourse(command.ToCourseId, CurrentUserIdOrDefault));

            var clonedAssignmentForms = assignmentForms
                .Select(p =>
                {
                    var newAssignmentId = clonedAssignmentsMapByOldId[p.Id].Id;
                    return p.Clone(newAssignmentId);
                })
                .ToList();
            var clonedAssignmentFormQuestions = assignmentFormQuestions
                .Select(p =>
                {
                    var newAssignmentId = clonedAssignmentsMapByOldId[p.QuizAssignmentFormId].Id;
                    return p.Clone(newAssignmentId);
                })
                .ToList();

            // Insert contents to db
            await _sectionCudLogic.InsertMany(clonedSectionsMapByOldId.Select(p => p.Value).ToList(), cancellationToken);
            await _lectureCudLogic.InsertMany(clonedLectureMapByOldId.Select(p => p.Value).ToList(), clonedContentLectures.ToList(), cancellationToken);
            await _assignmentCudLogic.InsertMany(clonedAssignmentsMapByOldId.Select(p => p.Value).ToList(), clonedAssignmentForms, clonedAssignmentFormQuestions, cancellationToken);

            await _processPostSavingContentLogic.Execute(toCourse, null, CurrentUserIdOrDefault, cancellationToken);
        }

        private async Task<int> GetMaxOrderNumberOfCourseAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var sectionOrders = await _readSectionRepository
                .GetAll()
                .Where(_ => _.CourseId == courseId && _.ClassRunId == null).Select(p => p.Order)
                .ToListAsync(cancellationToken);
            var lectureOrders = await _readLectureRepository
                .GetAll()
                .Where(_ => _.CourseId == courseId && _.ClassRunId == null).Select(p => p.Order)
                .ToListAsync(cancellationToken);
            return sectionOrders.Union(lectureOrders).Max() + 1 ?? 0;
        }
    }
}
