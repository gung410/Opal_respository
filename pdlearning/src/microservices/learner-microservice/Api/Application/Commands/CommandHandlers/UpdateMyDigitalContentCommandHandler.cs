using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateMyDigitalContentCommandHandler : BaseCommandHandler<UpdateMyDigitalContentCommand>
    {
        private readonly IWriteMyDigitalContentLogic _myDigitalContentCudLogic;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;
        private readonly IReadOnlyRepository<MyDigitalContent> _readMyDigitalContentRepository;

        public UpdateMyDigitalContentCommandHandler(
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            IWriteMyDigitalContentLogic myDigitalContentCudLogic,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic,
            IReadOnlyRepository<MyDigitalContent> readMyCourseRepository) : base(unitOfWorkManager, userContext)
        {
            _readMyDigitalContentRepository = readMyCourseRepository;
            _myDigitalContentCudLogic = myDigitalContentCudLogic;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        protected override async Task HandleAsync(UpdateMyDigitalContentCommand command, CancellationToken cancellationToken)
        {
            var existingMyDigitalContent = await _readMyDigitalContentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.DigitalContentId == command.DigitalContentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingMyDigitalContent == null)
            {
                throw new EntityNotFoundException(typeof(MyDigitalContent), command.DigitalContentId);
            }

            ValidateStatus(command, existingMyDigitalContent);

            existingMyDigitalContent.Status = command.Status;
            existingMyDigitalContent.ChangedBy = CurrentUserId;
            existingMyDigitalContent.ReadDate = command.ReadDate;
            existingMyDigitalContent.ProgressMeasure = command.ProgressMeasure;

            // Remove task if the status of MyDigitalContent is Completed.
            if (command.Status == MyDigitalContentStatus.Completed)
            {
                existingMyDigitalContent.CompletedDate = Clock.Now;

                await _myOutstandingTaskCudLogic.DeleteDigitalContentTask(existingMyDigitalContent);
            }

            await _myDigitalContentCudLogic.Update(existingMyDigitalContent);
        }

        private bool ValidateStatus(UpdateMyDigitalContentCommand command, MyDigitalContent existingMyDigitalContent)
        {
            if (existingMyDigitalContent.Status == MyDigitalContentStatus.InProgress && command.Status == MyDigitalContentStatus.Completed)
            {
                return true;
            }

            throw new InvalidStatusException();
        }
    }
}
