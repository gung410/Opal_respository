using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Models;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Entities.Abstractions;
using Microservice.Course.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class MoveContentUpOrDownCommandHandler : BaseCommandHandler<MoveContentUpOrDownCommand>
    {
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly EnsureCanSaveContentLogic _ensureCanSaveContentLogic;
        private readonly SectionCudLogic _sectionCudLogic;
        private readonly LectureCudLogic _lectureCudLogic;

        public MoveContentUpOrDownCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            ProcessPostSavingContentLogic processPostSavingContentLogic,
            EnsureCanSaveContentLogic ensureCanSaveContentLogic,
            SectionCudLogic sectionCudLogic,
            LectureCudLogic lectureCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSectionRepository = readSectionRepository;
            _readLectureRepository = readLectureRepository;
            _processPostSavingContentLogic = processPostSavingContentLogic;
            _ensureCanSaveContentLogic = ensureCanSaveContentLogic;
            _sectionCudLogic = sectionCudLogic;
            _lectureCudLogic = lectureCudLogic;
        }

        protected override async Task HandleAsync(MoveContentUpOrDownCommand command, CancellationToken cancellationToken)
        {
            var sectionOrLecture = command.Type == MovementContentType.Section
                ? (BaseOrderableContent)await _readSectionRepository.GetAsync(command.Id)
                : await _readLectureRepository.GetAsync(command.Id);
            var (items, course, classrun) = await GetSameLevelItems(sectionOrLecture, cancellationToken);

            if (command.Direction == MovementDirection.Up)
            {
                await MoveItemUp(sectionOrLecture, items, cancellationToken);
            }
            else
            {
                await MoveItemDown(sectionOrLecture, items, cancellationToken);
            }

            await _processPostSavingContentLogic.Execute(course, classrun, CurrentUserIdOrDefault, cancellationToken);
        }

        private async Task<ValueTuple<List<IOrderable>, CourseEntity, ClassRun>> GetSameLevelItems(BaseContent sectionOrLecture, CancellationToken cancellationToken)
        {
            var items = new List<IOrderable>();
            CourseEntity course;
            ClassRun classRun;

            switch (sectionOrLecture)
            {
                case Section section:
                    {
                        (course, classRun) = await _ensureCanSaveContentLogic.Execute(section.CourseId, section.ClassRunId, sectionOrLecture);
                        var lecturesSameLevel = await _readLectureRepository.GetAll()
                            .Where(_ => _.CourseId == section.CourseId && _.SectionId == null && _.ClassRunId == section.ClassRunId)
                            .ToListAsync(cancellationToken);

                        var sectionsSameLevel = await _readSectionRepository.GetAll()
                            .Where(_ => _.CourseId == section.CourseId && _.ClassRunId == section.ClassRunId)
                            .ToListAsync(cancellationToken);

                        items.AddRange(lecturesSameLevel.OfType<IOrderable>().Union(sectionsSameLevel));
                        break;
                    }

                case Lecture lecture:
                    {
                        (course, classRun) = await _ensureCanSaveContentLogic.Execute(lecture.CourseId, lecture.ClassRunId, sectionOrLecture);
                        var lecturesSameLevel = await _readLectureRepository.GetAll()
                            .Where(_ => _.CourseId == lecture.CourseId && _.ClassRunId == lecture.ClassRunId && _.SectionId == lecture.SectionId)
                            .ToListAsync(cancellationToken);

                        var sectionsSameLevel = await _readSectionRepository.GetAll()
                            .Where(_ => _.CourseId == lecture.CourseId && _.ClassRunId == lecture.ClassRunId)
                            .ToListAsync(cancellationToken);
                        items.AddRange(lecturesSameLevel.OfType<IOrderable>().Union(sectionsSameLevel));
                        break;
                    }

                default:
                    throw new GeneralException("Item must be either a lecture or section");
            }

            return (items.OrderBy(p => p.Order).ToList(), course, classRun);
        }

        private async Task MoveItemDown(IOrderable item, List<IOrderable> items, CancellationToken cancellationToken)
        {
            if (item.Order >= 0)
            {
                var nextItem = items.FirstOrDefault(_ => _.Order > item.Order);
                if (nextItem != null)
                {
                    await SwapOrder(item, nextItem, cancellationToken);
                }
            }
        }

        private async Task MoveItemUp(IOrderable item, List<IOrderable> items, CancellationToken cancellationToken)
        {
            if (item.Order > 0)
            {
                var previousItem = items.ReverseList().FirstOrDefault(_ => _.Order < item.Order);
                if (previousItem != null)
                {
                    await SwapOrder(item, previousItem, cancellationToken);
                }
            }
        }

        private async Task SwapOrder(IOrderable item, IOrderable secondItem, CancellationToken cancellationToken)
        {
            var order = item.Order;

            item.Order = secondItem.Order;
            secondItem.Order = order;
            await SaveItem(item, cancellationToken);
            await SaveItem(secondItem, cancellationToken);
        }

        private async Task SaveItem(IOrderable item, CancellationToken cancellationToken)
        {
            switch (item)
            {
                case Section section:
                    await _sectionCudLogic.Update(section, cancellationToken);
                    break;
                case Lecture lecture:
                    await _lectureCudLogic.Update(lecture, null, cancellationToken);
                    break;
            }
        }
    }
}
