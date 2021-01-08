using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ExtractContentUrlCommandHandler : BaseCommandHandler<ExtractContentUrlCommand>
    {
        private const int BatchSize = 10;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly ExtractCourseUrlLogic _extractCourseUrlLogic;
        private readonly IUnitOfWorkManager _uowManager;

        public ExtractContentUrlCommandHandler(
             IUnitOfWorkManager unitOfWorkManager,
             ExtractCourseUrlLogic extractContentUrlLogic,
             IUserContext userContext,
             IReadOnlyRepository<Lecture> readLectureRepository,
             IReadOnlyRepository<Assignment> readAssignmentRepository,
             IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = readLectureRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _extractCourseUrlLogic = extractContentUrlLogic;
            _uowManager = unitOfWorkManager;
        }

        protected override async Task HandleAsync(ExtractContentUrlCommand command, CancellationToken cancellationToken)
        {
            if (command.CourseId != null)
            {
                await ExtractBrokenLinkForCourseAsync(command.CourseId.Value, cancellationToken);
            }
            else if (command.ClassrunId != null)
            {
                await ExtractBrokenLinkForClassrunAsync(command.ClassrunId.Value, cancellationToken);
            }
            else if (command.AssignmentId != null)
            {
                await ExtractBrokenLinkForAssignmentAsync(command.AssignmentId.Value, cancellationToken);
            }
            else if (command.LectureId != null)
            {
                await ExtractBrokenLinkForLectureAsync(command.LectureId.Value, cancellationToken);
            }
            else
            {
                await ExtractBrokenLinkForUnExtractedContentsAsync(cancellationToken);
            }
        }

        private async Task ExtractBrokenLinkForCourseAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var lectures = await _readLectureRepository.GetAllListAsync(p => p.CourseId == courseId && p.ClassRunId == null);
            var assignments = await _readAssignmentRepository.GetAllListAsync(p => p.CourseId == courseId && p.ClassRunId == null);

            await _extractCourseUrlLogic.ExtractContentUrl(lectures.Select(p => p.Id).ToList(), ContentType.Lecture, cancellationToken);
            await _extractCourseUrlLogic.ExtractContentUrl(assignments.Select(p => p.Id).ToList(), ContentType.Assignment, cancellationToken);
        }

        private async Task ExtractBrokenLinkForClassrunAsync(Guid classrunId, CancellationToken cancellationToken)
        {
            var lectures = await _readLectureRepository.GetAllListAsync(p => p.ClassRunId == classrunId);
            var assignments = await _readAssignmentRepository.GetAllListAsync(p => p.ClassRunId == classrunId);

            await _extractCourseUrlLogic.ExtractContentUrl(lectures.Select(p => p.Id).ToList(), ContentType.Lecture, cancellationToken);
            await _extractCourseUrlLogic.ExtractContentUrl(assignments.Select(p => p.Id).ToList(), ContentType.Assignment, cancellationToken);
        }

        private async Task ExtractBrokenLinkForAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken)
        {
            var assignment = await _readAssignmentRepository.GetAsync(assignmentId);
            if (assignment == null)
            {
                return;
            }

            await _extractCourseUrlLogic.ExtractContentUrl(new List<Guid> { assignment.Id }, ContentType.Assignment, cancellationToken);
        }

        private async Task ExtractBrokenLinkForLectureAsync(Guid lectureId, CancellationToken cancellationToken)
        {
            var lecture = await _readLectureRepository.GetAsync(lectureId);
            if (lecture == null)
            {
                return;
            }

            await _extractCourseUrlLogic.ExtractContentUrl(new List<Guid> { lecture.Id }, ContentType.Lecture, cancellationToken);
        }

        private async Task ExtractBrokenLinkForUnExtractedContentsAsync(CancellationToken cancellationToken)
        {
            bool continueToExtract = true;
            int skipCount = 0;

            while (continueToExtract)
            {
                var contents = await TakeNotExtractedContent(skipCount, cancellationToken);
                if (!contents.Any())
                {
                    break;
                }

                await _uowManager.StartNewTransactionAsync(async () =>
                {
                    await _extractCourseUrlLogic.ExtractContentUrl(
                        contents.Where(p => p.Type == ContentType.Lecture).Select(p => p.Id).ToList(),
                        ContentType.Lecture,
                        cancellationToken);
                    await _extractCourseUrlLogic.ExtractContentUrl(
                        contents.Where(p => p.Type == ContentType.Assignment).Select(p => p.Id).ToList(),
                        ContentType.Assignment,
                        cancellationToken);
                });

                skipCount += BatchSize;
                continueToExtract = contents.Count == BatchSize;
            }
        }

        private async Task<List<ContentItem>> TakeNotExtractedContent(int skip, CancellationToken cancellationToken)
        {
            var unExtractedLectures = await _readLectureRepository
                .GetAll()
                .Skip(skip)
                .ToListAsync(cancellationToken);

            var unExtractedAssignments = await _readAssignmentRepository
                .GetAll()
                .Skip(skip)
                .ToListAsync(cancellationToken);

            return unExtractedLectures
                .Select(p => ContentItem.CreateForLecture(p))
                .Union(unExtractedAssignments.Select(p => ContentItem.CreateForAssignment(p)))
                .Take(BatchSize)
                .ToList();
        }
    }
}
