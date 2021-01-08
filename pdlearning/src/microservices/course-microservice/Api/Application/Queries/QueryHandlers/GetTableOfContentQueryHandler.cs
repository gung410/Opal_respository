using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetTableOfContentQueryHandler : BaseQueryHandler<GetTableOfContentQuery, List<ContentItem>>
    {
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;

        public GetTableOfContentQueryHandler(
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSectionRepository = readSectionRepository;
            _readLectureRepository = readLectureRepository;
            _readLectureContentRepository = readLectureContentRepository;
            _readAssignmentRepository = readAssignmentRepository;
        }

        protected override async Task<List<ContentItem>> HandleAsync(
            GetTableOfContentQuery query,
            CancellationToken cancellationToken)
        {
            var sections = await _readSectionRepository.GetAllListAsync(_ => _.CourseId == query.CourseId && _.ClassRunId == query.ClassRunId);
            var assignments = await _readAssignmentRepository
                .GetAll()
                .Where(_ => _.CourseId == query.CourseId && _.ClassRunId == query.ClassRunId)
                .OrderBy(p => p.CreatedDate)
                .ToListAsync(cancellationToken);

            var (lectures, lectureToContentMap) = await GetLecturesInfo(query);

            var courseSectionChilds = GetCourseSectionChilds(query, sections, lectures, lectureToContentMap);
            var courseLectureChilds = lectures
                .Where(p => p.SectionId == null)
                .Select(p => ContentItem.CreateForLecture(p, lectureToContentMap.ContainsKey(p.Id) ? lectureToContentMap[p.Id] : null, query.IncludeAdditionalInfo));
            var courseAssignment = assignments
                .Select(p => ContentItem.CreateForAssignment(p, query.IncludeAdditionalInfo));

            return courseSectionChilds
                .Concat(courseLectureChilds)
                .OrderBy(p => p.Order)
                .Concat(courseAssignment)
                .ToList();
        }

        private static IEnumerable<ContentItem> GetCourseSectionChilds(GetTableOfContentQuery query, List<Section> sections, List<Lecture> lectures, Dictionary<Guid, LectureContent> lectureToContentMap)
        {
            var sectionIdToLecturesMap = lectures.Where(p => p.SectionId != null).GroupBy(p => p.SectionId.Value).ToDictionary(p => p.Key, p => p.OrderBy(_ => _.Order).ToList());
            var courseSectionChilds = sections
                .Select(p => ContentItem.CreateForSection(p, sectionIdToLecturesMap.ContainsKey(p.Id) ? sectionIdToLecturesMap[p.Id] : null, lectureToContentMap, query.IncludeAdditionalInfo));
            return courseSectionChilds;
        }

        private async Task<Dictionary<Guid, LectureContent>> GetLectureToContentMap(List<Lecture> lectures)
        {
            var lectureIds = lectures.Select(_ => _.Id);
            var lectureContents = await _readLectureContentRepository.GetAllListAsync(_ => lectureIds.Contains(_.LectureId));
            var lectureToLectureContentMap = lectureContents.GroupBy(p => p.LectureId).ToDictionary(p => p.Key, p => p.ToList().FirstOrDefault());
            return lectureToLectureContentMap;
        }

        private async Task<Tuple<List<Lecture>, Dictionary<Guid, LectureContent>>> GetLecturesInfo(GetTableOfContentQuery query)
        {
            var lectures = await _readLectureRepository.GetAllListAsync(_ =>
                _.CourseId == query.CourseId
                && (string.IsNullOrEmpty(query.SearchText) || EF.Functions.FreeText(_.LectureName, query.SearchText))
                && _.ClassRunId == query.ClassRunId);

            var lectureToContentMap = await GetLectureToContentMap(lectures);
            return Tuple.Create(lectures, lectureToContentMap);
        }
    }
}
