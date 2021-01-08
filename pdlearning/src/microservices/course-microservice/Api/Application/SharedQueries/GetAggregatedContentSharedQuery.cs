using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetAggregatedECertificateTemplateSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<ECertificateTemplate> _readCertificateTemplateRepository;

        public GetAggregatedECertificateTemplateSharedQuery(IReadOnlyRepository<ECertificateTemplate> readCertificateTemplateRepository)
        {
            _readCertificateTemplateRepository = readCertificateTemplateRepository;
        }

        public async Task<ECertificateTemplateAggregatedEntityModel> ById(Guid ecertificateTemplateId)
        {
            var ecertificateTemplate = await _readCertificateTemplateRepository.GetAsync(ecertificateTemplateId);

            var ecertificateLayout = ECertificateLayoutConstant.AllECertificateLayoutsInSystem.First(p => p.Id == ecertificateTemplate.ECertificateLayoutId);

            return ECertificateTemplateAggregatedEntityModel.New(ecertificateTemplate).WithLayout(ecertificateLayout);
        }

        public async Task<List<ECertificateTemplateAggregatedEntityModel>> ByIds(List<Guid> ecertificateTemplateIds)
        {
            var ecertificateTemplates = await _readCertificateTemplateRepository
                .GetAll()
                .Where(p => ecertificateTemplateIds.Contains(p.Id))
                .ToListAsync();

            var allECertificateLayoutsInSystemDic = ECertificateLayoutConstant.AllECertificateLayoutsInSystem.ToDictionary(p => p.Id);

            return ecertificateTemplates.SelectList(ecertificateTemplate => ECertificateTemplateAggregatedEntityModel.New(ecertificateTemplate).WithLayout(allECertificateLayoutsInSystemDic[ecertificateTemplate.ECertificateLayoutId]));
        }
    }

    public class GetAggregatedContentSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IReadOnlyRepository<QuizAssignmentForm> _readQuizAssignmentFormRepository;
        private readonly IReadOnlyRepository<QuizAssignmentFormQuestion> _readQuizAssignmentFormQuestionRepository;

        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;

        public GetAggregatedContentSharedQuery(
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IReadOnlyRepository<QuizAssignmentForm> readQuizAssignmentFormRepository,
            IReadOnlyRepository<QuizAssignmentFormQuestion> readQuizAssignmentFormQuestionRepository)
        {
            _readLectureRepository = readLectureRepository;
            _readLectureContentRepository = readLectureContentRepository;
            _readUserRepository = readUserRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _readQuizAssignmentFormRepository = readQuizAssignmentFormRepository;
            _readQuizAssignmentFormQuestionRepository = readQuizAssignmentFormQuestionRepository;
        }

        public async Task<List<LectureAggregatedEntityModel>> LectureByIds(List<Guid> ids, bool includeOwner, CancellationToken cancellationToken)
        {
            var lectureContentQuery = _readLectureContentRepository.GetAll();
            var lecturesQuery = _readLectureRepository
                .GetAll()
                .Where(p => ids.Contains(p.Id));
            var lectures = await lecturesQuery
                .Join(
                    lectureContentQuery,
                    p => p.Id,
                    p => p.LectureId,
                    (lecture, lectureContent) => new { lecture, lectureContent })
                .ToListAsync(cancellationToken);

            var ownersDic = includeOwner
                ? await _readUserRepository
                    .GetAll()
                    .Join(lecturesQuery, p => p.Id, p => p.CreatedBy, (user, lecture) => user)
                    .Distinct()
                    .ToDictionaryAsync(p => p.Id, cancellationToken)
                : new Dictionary<Guid, CourseUser>();

            var models = lectures
                .Select(p => LectureAggregatedEntityModel.Create(p.lecture, p.lectureContent, ownersDic.GetValueOrDefault(p.lecture.CreatedBy)))
                .ToList();

            return models;
        }

        public Task<List<AssignmentAggregatedEntityModel>> AssignmentByIds(List<Guid> ids, bool includeOwner, CancellationToken cancellationToken)
        {
            var assignmentsQuery = _readAssignmentRepository
                .GetAll()
                .Where(p => ids.Contains(p.Id));

            return AssignmentByQuery(assignmentsQuery, true, includeOwner, cancellationToken);
        }

        public async Task<List<AssignmentAggregatedEntityModel>> AssignmentByQuery(
            IQueryable<Assignment> assignmentsQuery,
            bool includeQuizForm,
            bool includeOwner,
            CancellationToken cancellationToken)
        {
            var assignmentQuizFormsQuery = assignmentsQuery
                .Join(_readQuizAssignmentFormRepository.GetAll(), p => p.Id, p => p.Id, (assignment, quizForm) => quizForm);

            var assignments = await assignmentsQuery.ToListAsync(cancellationToken);

            if (!includeQuizForm)
            {
                return assignments
                    .Select(p => AssignmentAggregatedEntityModel.Create(p, null, null))
                    .ToList();
            }

            var assignmentQuizFormQuestions = (await assignmentQuizFormsQuery
                .Join(_readQuizAssignmentFormQuestionRepository.GetAll(), p => p.Id, p => p.QuizAssignmentFormId, (form, question) => question)
                .ToListAsync(cancellationToken))
                .GroupBy(p => p.QuizAssignmentFormId);

            var assignmentQuizFormsDic = await assignmentQuizFormsQuery
                .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

            var assignmentQuizFormQuestionsDic = assignmentQuizFormQuestions
                .ToDictionary(p => p.Key, p => p.ToList());

            var ownersDic = includeOwner
                ? await _readUserRepository
                    .GetAll()
                    .Join(assignmentsQuery, p => p.Id, p => p.CreatedBy, (user, assignment) => user)
                    .Distinct()
                    .ToDictionaryAsync(p => p.Id, cancellationToken)
                : new Dictionary<Guid, CourseUser>();

            var models = assignments
                .Select(p => AssignmentAggregatedEntityModel.Create(
                    p,
                    assignmentQuizFormsDic.GetValueOrDefault(p.Id),
                    assignmentQuizFormQuestionsDic.GetValueOrDefault(p.Id),
                    ownersDic.GetValueOrDefault(p.CreatedBy)))
                .ToList();

            return models;
        }

        public async Task<List<IAggregatedContentEntityModel>> ByIdsAndType(
            List<Guid> ids,
            ContentType contentType,
            bool includeOwner,
            CancellationToken cancellationToken)
        {
            if (contentType == ContentType.Lecture)
            {
                return (await LectureByIds(ids, includeOwner, cancellationToken)).Select(p => (IAggregatedContentEntityModel)p).ToList();
            }

            if (contentType == ContentType.Assignment)
            {
                return (await AssignmentByIds(ids, includeOwner, cancellationToken)).Select(p => (IAggregatedContentEntityModel)p).ToList();
            }

            return new List<IAggregatedContentEntityModel>();
        }
    }
}
