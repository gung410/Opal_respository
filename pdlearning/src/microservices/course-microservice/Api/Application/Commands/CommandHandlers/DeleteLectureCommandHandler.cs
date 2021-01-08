using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteLectureCommandHandler : BaseCommandHandler<DeleteLectureCommand>
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly ExtractCourseUrlLogic _extractCourseUrlLogic;
        private readonly EnsureCanSaveContentLogic _ensureCanSaveContentLogic;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly LectureCudLogic _lectureCudLogic;
        private readonly SectionCudLogic _sectionCudLogic;

        public DeleteLectureCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            ExtractCourseUrlLogic extractContentUrlLogic,
            LectureCudLogic lectureCudLogic,
            SectionCudLogic sectionCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanSaveContentLogic ensureCanSaveContentLogic,
            ProcessPostSavingContentLogic processPostSavingContentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = readLectureRepository;
            _readSectionRepository = readSectionRepository;
            _extractCourseUrlLogic = extractContentUrlLogic;
            _ensureCanSaveContentLogic = ensureCanSaveContentLogic;
            _processPostSavingContentLogic = processPostSavingContentLogic;
            _lectureCudLogic = lectureCudLogic;
            _sectionCudLogic = sectionCudLogic;
        }

        protected override async Task HandleAsync(DeleteLectureCommand command, CancellationToken cancellationToken)
        {
            var lecture = await _readLectureRepository.GetAsync(command.Id);
            var (course, classRun) = await _ensureCanSaveContentLogic.Execute(lecture.CourseId, lecture.ClassRunId, lecture);

            await UpdateSiblingsOrderAsync(lecture.CourseId, lecture.SectionId, lecture.Order, lecture.Id, cancellationToken);

            await _lectureCudLogic.Delete(lecture, cancellationToken);

            await _processPostSavingContentLogic.Execute(course, classRun, CurrentUserIdOrDefault, cancellationToken);

            await _extractCourseUrlLogic.DeleteExtractedUrls(new List<Guid> { lecture.Id });
        }

        private async Task UpdateSiblingsOrderAsync(Guid courseId, Guid? sectionId, int? fromOrder, Guid excludeId, CancellationToken cancellationToken)
        {
            // get all lectures have order below deleting lecture order
            var lecturesLowerLevel = await _readLectureRepository.GetAll()
                    .Where(_ => _.CourseId == courseId && _.SectionId == sectionId && _.Order.Value > fromOrder && _.Id != excludeId)
                    .ToListAsync(cancellationToken);

            // get all sections have order below deleting lecture order
            var sectionsLowerLevel = sectionId == null
                ? await _readSectionRepository.GetAll()
                    .Where(_ => _.CourseId == courseId && _.Order.Value > fromOrder && _.Id != excludeId)
                    .ToListAsync(cancellationToken)
                : new List<Section>();

            // merge two list
            var sortedSiblings =
                lecturesLowerLevel.OfType<IOrderable>().Union(sectionsLowerLevel);

            // move all lectures and sections order up
            foreach (var sortedSibling in sortedSiblings)
            {
                sortedSibling.Order -= 1;
            }

            await _sectionCudLogic.UpdateMany(sectionsLowerLevel, cancellationToken);
            await _lectureCudLogic.UpdateMany(lecturesLowerLevel, cancellationToken);
        }
    }
}
