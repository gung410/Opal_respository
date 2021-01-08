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
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CloneContentForClassRunCommandHandler : BaseCommandHandler<CloneContentForClassRunCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IReadOnlyRepository<QuizAssignmentForm> _readQuizAssignmentFormRepository;
        private readonly IReadOnlyRepository<QuizAssignmentFormQuestion> _readQuizAssignmentFormQuestionRepository;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly LectureCudLogic _lectureCudLogic;
        private readonly SectionCudLogic _sectionCudLogic;
        private readonly AssignmentCudLogic _assignmentCudLogic;

        public CloneContentForClassRunCommandHandler(
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
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
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
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
        }

        protected override async Task HandleAsync(CloneContentForClassRunCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(command.ClassRunId);

            EnsureBusinessLogicValid(aggregatedClassRun.Course.ValidateNotArchived());

            EnsureValidPermission(aggregatedClassRun.ClassRun.HasContentCustomizationPermission(
                aggregatedClassRun.Course,
                CurrentUserId,
                CurrentUserRoles,
                _readCourseRepository.GetHasAdminRightChecker(aggregatedClassRun.Course, AccessControlContext)));

            // Get Course Contents
            var sections = await _readSectionRepository.GetAllListAsync(_ => _.CourseId == aggregatedClassRun.Course.Id && _.ClassRunId == null);
            var lectures = await _readLectureRepository.GetAllListAsync(_ => _.CourseId == aggregatedClassRun.Course.Id && _.ClassRunId == null);
            var lectureIds = lectures.Select(x => x.Id);
            var lectureContents = await _readLectureContentRepository.GetAllListAsync(_ => lectureIds.Contains(_.LectureId));
            var assignments = await _readAssignmentRepository.GetAllListAsync(_ => _.CourseId == aggregatedClassRun.Course.Id && _.ClassRunId == null);
            var assignmentIds = assignments.Select(x => x.Id);
            var assignmentForms = await _readQuizAssignmentFormRepository.GetAllListAsync(_ => assignmentIds.Contains(_.Id));
            var assignmentFormQuestions = await _readQuizAssignmentFormQuestionRepository.GetAllListAsync(_ => assignmentIds.Contains(_.QuizAssignmentFormId));

            // Clone contents and update the data to match for class run
            var clonedSectionsMapByOldId = sections.ToDictionary(p => p.Id, p => p.CloneForClassRun(command.ClassRunId, CurrentUserIdOrDefault));
            var clonedLectureMapByOldId = lectures.ToDictionary(p => p.Id, p =>
            {
                var newSectionId = p.SectionId == null || !clonedSectionsMapByOldId.ContainsKey(p.SectionId.Value) ? null : (Guid?)clonedSectionsMapByOldId[p.SectionId.Value].Id;
                return p.CloneForClassRun(command.ClassRunId, CurrentUserIdOrDefault, newSectionId);
            });
            var clonedLectures = lectureContents.Select(p =>
            {
                var newLectureId = clonedLectureMapByOldId[p.LectureId].Id;
                return p.Clone(CurrentUserIdOrDefault, newLectureId);
            });

            var clonedAssignmentsMapByOldId = assignments.ToDictionary(p => p.Id, p => p.CloneForClassRun(command.ClassRunId, CurrentUserIdOrDefault));

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
            await _lectureCudLogic.InsertMany(clonedLectureMapByOldId.Select(p => p.Value).ToList(), clonedLectures.ToList(), cancellationToken);
            await _assignmentCudLogic.InsertMany(clonedAssignmentsMapByOldId.Select(p => p.Value).ToList(), clonedAssignmentForms, clonedAssignmentFormQuestions, cancellationToken);

            aggregatedClassRun.ClassRun.ContentStatus = ContentStatus.Draft;

            await _processPostSavingContentLogic.Execute(aggregatedClassRun.Course, aggregatedClassRun.ClassRun, CurrentUserIdOrDefault, cancellationToken);
        }
    }
}
