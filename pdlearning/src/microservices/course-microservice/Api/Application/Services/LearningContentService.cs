using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class LearningContentService : BaseApplicationService
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IReadOnlyRepository<QuizAssignmentForm> _readQuizAssignmentFormRepository;
        private readonly IReadOnlyRepository<QuizAssignmentFormQuestion> _readQuizAssignmentFormQuestionRepository;

        public LearningContentService(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IReadOnlyRepository<QuizAssignmentForm> readQuizAssignmentFormRepository,
            IReadOnlyRepository<QuizAssignmentFormQuestion> readQuizAssignmentFormQuestionRepository,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
            _readClassRunRepository = readClassRunRepository;
            _readSectionRepository = readSectionRepository;
            _readLectureRepository = readLectureRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _readLectureContentRepository = readLectureContentRepository;
            _readQuizAssignmentFormRepository = readQuizAssignmentFormRepository;
            _readQuizAssignmentFormQuestionRepository = readQuizAssignmentFormQuestionRepository;
        }

        public Task<List<ContentItem>> GetTableOfContent(Guid id, Guid? classRunId, string searchText, bool includeAdditionalInfo)
        {
            return ThunderCqrs.SendQuery(new GetTableOfContentQuery
            {
                CourseId = id,
                ClassRunId = classRunId,
                SearchText = searchText,
                IncludeAdditionalInfo = includeAdditionalInfo
            });
        }

        public Task<LectureModel> GetLectureById(Guid lectureId)
        {
            return ThunderCqrs.SendQuery(new GetLectureDetailsQuery
            {
                LectureId = lectureId
            });
        }

        public Task<SectionModel> GetSectionById(Guid sectionId)
        {
            return ThunderCqrs.SendQuery(new GetSectionByIdQuery
            {
                Id = sectionId
            });
        }

        public async Task<SectionModel> CreateOrUpdateSection(CreateOrUpdateSectionRequest request)
        {
            var command = new CreateOrUpdateSectionCommand
            {
                Id = request.Data.Id ?? Guid.NewGuid(),
                CreateOrUpdateRequest = request,
                IsCreateNew = !request.Data.Id.HasValue
            };

            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetSectionByIdQuery { Id = command.Id });
        }

        public Task DeleteSection(Guid courseId, Guid sectionId)
        {
            return ThunderCqrs.SendCommand(new DeleteSectionCommand { SectionId = sectionId, CourseId = courseId });
        }

        public async Task<LectureModel> SaveLecture(SaveLectureRequest request)
        {
            var command = request.ToCommand();
            await ThunderCqrs.SendCommand(command);

            await ThunderCqrs.SendCommand(new ExtractContentUrlCommand { LectureId = command.Id });
            return await ThunderCqrs.SendQuery(new GetLectureDetailsQuery { LectureId = command.Id });
        }

        public Task<List<Guid>> GetAllLectureIdsByCourseId(Guid courseId)
        {
            return ThunderCqrs.SendQuery(new GetAllLectureIdsBelongToCourseQuery { CourseId = courseId });
        }

        public Task DeleteLecture(Guid courseId, Guid lectureId)
        {
            return ThunderCqrs.SendCommand(new DeleteLectureCommand
            {
                Id = lectureId,
                CourseId = courseId
            });
        }

        public async Task<List<ContentItem>> ChangeContentOrder(ChangeContentOrderRequest request)
        {
            await ThunderCqrs.SendCommand(new MoveContentUpOrDownCommand
            {
                Id = request.Id,
                Direction = request.Direction,
                Type = request.Type,
                CourseId = request.CourseId,
                ClassRunId = request.ClassRunId
            });
            return await ThunderCqrs.SendQuery(new GetTableOfContentQuery
            {
                CourseId = request.CourseId,
                ClassRunId = request.ClassRunId,
                SearchText = null
            });
        }

        public Task<CourseIdMapListDigitalContentIdModel[]> GetAllDigitalContentIdsBelongTheseCourseIds(Guid[] listCourseIds)
        {
            return ThunderCqrs.SendQuery(new GetAllDigitalContentIdsBelongCourseIdsQuery { CourseIds = listCourseIds });
        }

        public Task<LectureIdMapNameModel[]> GetAllLectureNamesByListIds(Guid[] listLectureIds)
        {
            return ThunderCqrs.SendQuery(new GetAllLectureNamesByListIdsQuery { ListLectureIds = listLectureIds });
        }

        public async Task<List<ContentItem>> CloneContentForClassRun(CloneContentForClassRunRequest request)
        {
            await ThunderCqrs.SendCommand(new CloneContentForClassRunCommand
            {
                ClassRunId = request.ClassRunId
            });

            await ThunderCqrs.SendCommand(new ExtractContentUrlCommand
            {
                ClassrunId = request.ClassRunId,
            });

            return await ThunderCqrs.SendQuery(new GetTableOfContentQuery
            {
                CourseId = request.CourseId,
                ClassRunId = request.ClassRunId
            });
        }

        public async Task<List<ContentItem>> CloneContentForCourse(CloneContentForCourseRequest request)
        {
            await ThunderCqrs.SendCommand(new CloneContentForCourseCommand
            {
                FromCourseId = request.FromCourseId,
                ToCourseId = request.ToCourseId
            });

            await ThunderCqrs.SendCommand(new ExtractContentUrlCommand
            {
                CourseId = request.ToCourseId,
            });

            return await ThunderCqrs.SendQuery(new GetTableOfContentQuery
            {
                CourseId = request.ToCourseId
            });
        }

        public async Task ChangeCourseContentStatus(ChangeCourseContentStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeCourseContentStatusCommand
            {
                ContentStatus = request.ContentStatus,
                Ids = request.Ids
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.CourseContent,
                    StatusEnum = request.ContentStatus,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public async Task ChangeClassRunContentStatus(ChangeClassRunContentStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeClassRunContentStatusCommand
            {
                ContentStatus = request.ContentStatus,
                Ids = request.Ids
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.ClassRunContent,
                    StatusEnum = request.ContentStatus,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public Task<bool> HasReferenceToResource(HasReferenceToResourceRequest request)
        {
            return ThunderCqrs.SendQuery(new HasReferenceToResourceQuery
            {
                ResourceId = request.ResourceId
            });
        }

        public async Task<PagedResultDto<Guid>> MigrateContentNotification(MigrateContentNotificationRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var query = _readClassRunRepository
                    .GetAll()
                    .WhereIf(request.ClassRunIds != null && request.ClassRunIds.Any(), p => request.ClassRunIds.Contains(p.Id));

                var totalCount = await query.CountAsync();
                if (request.SkipCount < 0 || request.MaxResultCount <= 0)
                {
                    return new PagedResultDto<Guid>(totalCount);
                }

                var result = await query.Skip(request.SkipCount).Take(request.MaxResultCount).ToListAsync();

                var classRunIds = result.Select(x => x.Id).ToList();

                var sections = await _readSectionRepository
                    .GetAll()
                    .WhereIf(classRunIds != null && classRunIds.Any(), section => classRunIds.Contains(section.ClassRunId.Value))
                    .ToListAsync();

                await ThunderCqrs.SendEvents(sections.Select(p => new SectionChangeEvent(p, SectionChangeType.Updated, true)));

                var lectures = await _readLectureRepository
                    .GetAll()
                    .WhereIf(classRunIds != null && classRunIds.Any(), lecture => classRunIds.Contains(lecture.ClassRunId.Value))
                    .ToListAsync();

                var lectureIds = lectures.Select(x => x.Id);
                var lectureContentDic = await _readLectureContentRepository
                    .GetAll()
                    .Where(_ => lectureIds.Contains(_.LectureId))
                    .ToDictionaryAsync(x => x.LectureId, x => x);

                await ThunderCqrs.SendEvents(lectures.Select(p => new LectureChangeEvent(LectureModel.Create(p, lectureContentDic.GetValueOrDefault(p.Id, new LectureContent())), LectureChangeType.Updated, true)));

                var assignments = await _readAssignmentRepository
                    .GetAll()
                    .WhereIf(classRunIds != null && classRunIds.Any(), assignment => classRunIds.Contains(assignment.ClassRunId.Value))
                    .ToListAsync();

                var assignmentIds = assignments.Select(x => x.Id);
                var quizAssignmentFormDic = await _readQuizAssignmentFormRepository
                    .GetAll()
                    .Where(_ => assignmentIds.Contains(_.Id))
                    .ToDictionaryAsync(_ => _.Id, _ => _);
                var assignmentFormQuestions = await _readQuizAssignmentFormQuestionRepository.GetAllListAsync(_ => assignmentIds.Contains(_.QuizAssignmentFormId));

                var quizAssignmentFormQuestionDic = assignmentFormQuestions
                    .GroupBy(x => x.QuizAssignmentFormId)
                    .ToDictionary(x => x.Key, x => x.ToList());

                await ThunderCqrs.SendEvents(
                     assignments.Select(x =>
                         new AssignmentChangeEvent(
                             new AssignmentModel(
                                 x,
                                 new QuizAssignmentFormModel(
                                     quizAssignmentFormDic.GetValueOrDefault(x.Id, new QuizAssignmentForm()),
                                     quizAssignmentFormQuestionDic.GetValueOrDefault(x.Id, new List<QuizAssignmentFormQuestion>()),
                                     false)),
                             AssignmentChangeType.Created,
                             true)));

                return new PagedResultDto<Guid>(totalCount, classRunIds);
            });
        }
    }
}
