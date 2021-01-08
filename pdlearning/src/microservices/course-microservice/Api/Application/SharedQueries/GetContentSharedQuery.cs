using System;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetContentSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;

        public GetContentSharedQuery(
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository)
        {
            _readLectureRepository = readLectureRepository;
            _readAssignmentRepository = readAssignmentRepository;
        }

        public async Task<ContentItem> ById(
            Guid contentId,
            ContentType contentType)
        {
            if (contentType == ContentType.Lecture)
            {
                var lecture = await _readLectureRepository.FirstOrDefaultAsync(contentId);
                return lecture != null ? ContentItem.CreateForLecture(lecture) : null;
            }

            if (contentType == ContentType.Assignment)
            {
                var assignment = await _readAssignmentRepository.FirstOrDefaultAsync(contentId);
                return assignment != null ? ContentItem.CreateForAssignment(assignment) : null;
            }

            return null;
        }
    }
}
