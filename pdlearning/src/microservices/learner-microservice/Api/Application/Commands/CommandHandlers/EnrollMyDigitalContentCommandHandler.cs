using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class EnrollMyDigitalContentCommandHandler : BaseCommandHandler<EnrollMyDigitalContentCommand>
    {
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCud;
        private readonly IWriteMyDigitalContentLogic _myDigitalContentCudLogic;
        private readonly IReadOnlyRepository<MyDigitalContent> _readMyDigitalContentRepository;

        public EnrollMyDigitalContentCommandHandler(
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCud,
            IWriteMyDigitalContentLogic myDigitalContentCudLogic,
            IReadOnlyRepository<MyDigitalContent> readMyDigitalContentRepository) : base(unitOfWorkManager, userContext)
        {
            _myOutstandingTaskCud = myOutstandingTaskCud;
            _myDigitalContentCudLogic = myDigitalContentCudLogic;
            _readMyDigitalContentRepository = readMyDigitalContentRepository;
        }

        protected override async Task HandleAsync(EnrollMyDigitalContentCommand command, CancellationToken cancellationToken)
        {
            var myDigitalContent = await _readMyDigitalContentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.DigitalContentId == command.DigitalContentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (myDigitalContent != null && !myDigitalContent.IsNotStarted())
            {
                throw new EnrolledCourseException();
            }

            var newMyDigitalContent = new MyDigitalContent()
            {
                Id = command.Id,
                ProgressMeasure = 0,
                StartDate = Clock.Now,
                UserId = CurrentUserIdOrDefault,
                CreatedBy = CurrentUserIdOrDefault,
                DigitalContentId = command.DigitalContentId,
                Status = MyDigitalContentStatus.InProgress
            };

            await _myDigitalContentCudLogic.Insert(newMyDigitalContent);

            await _myOutstandingTaskCud.InsertDigitalContentTask(newMyDigitalContent);
        }
    }
}
