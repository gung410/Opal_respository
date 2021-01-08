using System;
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
    public class CreateOrUpdateSectionCommandHandler : BaseCommandHandler<CreateOrUpdateSectionCommand>
    {
        private readonly IReadOnlyRepository<Section> _readSectionRepository;
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly EnsureCanSaveContentLogic _ensureCanSaveContentLogic;
        private readonly ProcessPostSavingContentLogic _processPostSavingContentLogic;
        private readonly SectionCudLogic _sectionCudLogic;
        private readonly LectureCudLogic _lectureCudLogic;

        public CreateOrUpdateSectionCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Section> readSectionRepository,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanSaveContentLogic ensureCanSaveContentLogic,
            ProcessPostSavingContentLogic processPostSavingContentLogic,
            SectionCudLogic sectionCudLogic,
            LectureCudLogic lectureCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSectionRepository = readSectionRepository;
            _readLectureRepository = readLectureRepository;
            _ensureCanSaveContentLogic = ensureCanSaveContentLogic;
            _processPostSavingContentLogic = processPostSavingContentLogic;
            _sectionCudLogic = sectionCudLogic;
            _lectureCudLogic = lectureCudLogic;
        }

        protected override async Task HandleAsync(CreateOrUpdateSectionCommand command, CancellationToken cancellationToken)
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

        private async Task Update(CreateOrUpdateSectionCommand command, CancellationToken cancellationToken)
        {
            var request = command.CreateOrUpdateRequest;
            var section = await _readSectionRepository.GetAsync(command.Id);

            var (course, classrun) = await _ensureCanSaveContentLogic.Execute(command.CreateOrUpdateRequest.Data.CourseId, command.CreateOrUpdateRequest.Data.ClassRunId);

            section.ChangedBy = CurrentUserIdOrDefault;
            section.ChangedDate = Clock.Now;
            section.Description = request.Data.Description;
            section.CourseId = request.Data.CourseId;
            section.ClassRunId = request.Data.ClassRunId;
            section.Title = request.Data.Title;
            section.Order = request.Data.Order;
            section.CreditsAward = request.Data.CreditsAward;

            await UpdateOrderOfSiblings(request.Data.CourseId, request.Data.Order, section.Id, cancellationToken);
            await _processPostSavingContentLogic.Execute(course, classrun, CurrentUserIdOrDefault, cancellationToken);
        }

        private async Task UpdateOrderOfSiblings(Guid courseId, int fromOrder, Guid excludeId, CancellationToken cancellationToken)
        {
            var lecturesSameLevel = await _readLectureRepository.GetAll()
                .Where(_ => _.CourseId == courseId && _.SectionId == null && _.Order.Value >= fromOrder && _.Id != excludeId).ToListAsync(cancellationToken);

            var sectionsSameLevel = await _readSectionRepository.GetAll().
                Where(_ => _.CourseId == courseId && _.Order.Value >= fromOrder && _.Id != excludeId).ToListAsync(cancellationToken);

            var sortedSiblings =
                lecturesSameLevel.OfType<IOrderable>().Union(sectionsSameLevel);

            foreach (var sortedSibling in sortedSiblings)
            {
                sortedSibling.Order = sortedSibling.Order + 1;
            }

            await _sectionCudLogic.UpdateMany(sectionsSameLevel, cancellationToken);
            await _lectureCudLogic.UpdateMany(lecturesSameLevel, cancellationToken);
        }

        private async Task CreateNew(CreateOrUpdateSectionCommand command, CancellationToken cancellationToken)
        {
            var (course, classrun) = await _ensureCanSaveContentLogic.Execute(command.CreateOrUpdateRequest.Data.CourseId, command.CreateOrUpdateRequest.Data.ClassRunId);

            var request = command.CreateOrUpdateRequest;
            var section = new Section
            {
                Id = command.Id,
                CreatedBy = CurrentUserIdOrDefault,
                CreatedDate = Clock.Now,
                Description = request.Data.Description,
                CourseId = request.Data.CourseId,
                ClassRunId = request.Data.ClassRunId,
                Title = request.Data.Title,
                Order = request.Data.Order,
                CreditsAward = request.Data.CreditsAward
            };

            await _sectionCudLogic.Insert(section, cancellationToken);

            await UpdateOrderOfSiblings(request.Data.CourseId, request.Data.Order, Guid.Empty, cancellationToken);

            await _processPostSavingContentLogic.Execute(course, classrun, CurrentUserIdOrDefault, cancellationToken);
        }
    }
}
