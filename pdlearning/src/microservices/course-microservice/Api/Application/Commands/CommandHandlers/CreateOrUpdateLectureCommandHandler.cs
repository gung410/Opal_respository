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
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CreateOrUpdateLectureCommandHandler : BaseCommandHandler<CreateOrUpdateLectureCommand>
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly EnsureCanSaveContentLogic _ensureCanSaveContentLogic;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly LectureCudLogic _lectureCudLogic;
        private readonly SectionCudLogic _sectionCudLogic;

        public CreateOrUpdateLectureCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanSaveContentLogic ensureCanSaveContentLogic,
            ProcessPostSavingContentLogic processPostSavingContentLogic,
            LectureCudLogic lectureCudLogic,
            SectionCudLogic sectionCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = readLectureRepository;
            _readSectionRepository = readSectionRepository;
            _readLectureContentRepository = readLectureContentRepository;
            _ensureCanSaveContentLogic = ensureCanSaveContentLogic;
            _processPostSavingContentLogic = processPostSavingContentLogic;
            _lectureCudLogic = lectureCudLogic;
            _sectionCudLogic = sectionCudLogic;
        }

        protected override async Task HandleAsync(CreateOrUpdateLectureCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreateNew)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task Update(CreateOrUpdateLectureCommand command, CancellationToken cancellationToken)
        {
            var lecture = await _readLectureRepository.GetAsync(command.Id);
            var lectureContent = await _readLectureContentRepository.FirstOrDefaultAsync(_ => _.LectureId == command.Id);

            var (course, classrun) = await _ensureCanSaveContentLogic.Execute(command.CourseId, command.ClassRunId);

            lecture.SectionId = command.SectionId;
            lecture.ClassRunId = command.ClassRunId;
            lecture.Description = command.Description;
            lecture.LectureName = command.LectureName;
            lecture.LectureIcon = command.LectureIcon;
            lecture.Order = command.Order;
            lecture.ChangedDate = Clock.Now;
            lecture.ChangedBy = CurrentUserIdOrDefault;

            lectureContent.MimeType = command.MimeType;
            lectureContent.ResourceId = command.ResourceId;
            lectureContent.Type = command.Type;
            lectureContent.Value = command.GetDecodedValue();
            lectureContent.ChangedDate = Clock.Now;
            lectureContent.ChangedBy = CurrentUserIdOrDefault;
            if (command.QuizConfig != null)
            {
                lectureContent.UpdateQuizConfig(command.QuizConfig.ToLectureQuizConfig());
            }

            if (command.DigitalContentConfig != null)
            {
                lectureContent.UpdateDigitalContentConfig(command.DigitalContentConfig.ToLectureDigitalContentConfig());
            }

            await _lectureCudLogic.Update(lecture, lectureContent, cancellationToken);

            if (command.Order.HasValue)
            {
                await UpdateOrderOfSiblings(command.CourseId, command.SectionId, command.Order.Value, lecture.Id);
            }

            await _processPostSavingContentLogic.Execute(course, classrun, CurrentUserIdOrDefault, cancellationToken);
        }

        private async Task UpdateOrderOfSiblings(Guid courseId, Guid? sectionId, int fromOrder, Guid excludeId = default, CancellationToken cancellationToken = default)
        {
            var lecturesSameLevel = await _readLectureRepository.GetAll()
                .Where(_ => _.CourseId == courseId && _.SectionId == sectionId && _.Order.Value >= fromOrder && _.Id != excludeId).ToListAsync(cancellationToken);

            var sectionsSameLevel = sectionId == null
                 ? await _readSectionRepository.GetAll().Where(_ => _.CourseId == courseId && _.Order.Value >= fromOrder && _.Id != excludeId).ToListAsync(cancellationToken)
                 : new List<Section>();

            var sortedSiblings =
                lecturesSameLevel.OfType<IOrderable>().Union(sectionsSameLevel);

            foreach (var sortedSibling in sortedSiblings)
            {
                sortedSibling.Order = sortedSibling.Order + 1;
            }

            await _sectionCudLogic.UpdateMany(sectionsSameLevel, cancellationToken);
            await _lectureCudLogic.UpdateMany(lecturesSameLevel, cancellationToken);
        }

        private async Task CreateNew(CreateOrUpdateLectureCommand command, CancellationToken cancellationToken)
        {
            var (course, classrun) = await _ensureCanSaveContentLogic.Execute(command.CourseId, command.ClassRunId);

            var lecture = new Lecture
            {
                Id = command.Id,
                CourseId = command.CourseId,
                ClassRunId = command.ClassRunId,
                SectionId = command.SectionId,
                Description = command.Description,
                LectureName = command.LectureName,
                LectureIcon = command.LectureIcon,
                Order = command.Order,
                CreatedDate = Clock.Now,
                CreatedBy = CurrentUserIdOrDefault
            };

            var lectureContent = new LectureContent
            {
                Id = Guid.NewGuid(),
                LectureId = command.Id,
                MimeType = command.MimeType,
                ResourceId = command.ResourceId,
                Type = command.Type,
                Value = command.GetDecodedValue(),
                CreatedDate = Clock.Now,
                CreatedBy = CurrentUserIdOrDefault
            };
            if (command.QuizConfig != null)
            {
                lectureContent.UpdateQuizConfig(command.QuizConfig.ToLectureQuizConfig());
            }

            if (command.DigitalContentConfig != null)
            {
                lectureContent.UpdateDigitalContentConfig(command.DigitalContentConfig.ToLectureDigitalContentConfig());
            }

            await _lectureCudLogic.Insert(lecture, lectureContent, cancellationToken);

            if (command.Order.HasValue)
            {
                await UpdateOrderOfSiblings(command.CourseId, command.SectionId, command.Order.Value);
            }

            await _processPostSavingContentLogic.Execute(course, classrun, CurrentUserIdOrDefault, cancellationToken);
        }
    }
}
