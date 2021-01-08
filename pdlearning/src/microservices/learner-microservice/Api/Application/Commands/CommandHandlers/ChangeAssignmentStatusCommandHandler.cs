using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class ChangeAssignmentStatusCommandHandler : BaseCommandHandler<ChangeAssignmentStatusCommand>
    {
        private readonly IRepository<MyAssignment> _myAssignmentRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public ChangeAssignmentStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MyAssignment> myAssignmentRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _myAssignmentRepository = myAssignmentRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(ChangeAssignmentStatusCommand command, CancellationToken cancellationToken)
        {
            var myAssignment = await _myAssignmentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.RegistrationId == command.RegistrationId)
                .Where(p => p.AssignmentId == command.AssignmentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (myAssignment == null)
            {
                throw new EntityNotFoundException(typeof(MyAssignment), command.AssignmentId);
            }

            var now = Clock.Now;
            myAssignment.ChangedDate = now;
            myAssignment.ChangedBy = CurrentUserId;
            myAssignment.Status = command.Status;

            if (command.Status == MyAssignmentStatus.Completed)
            {
                myAssignment.SubmittedDate = now;
            }

            await _myAssignmentRepository.UpdateAsync(myAssignment);

            await _thunderCqrs.SendEvent(
                new AssignmentChangeEvent(myAssignment, AssignmentChangeType.Updated),
                cancellationToken);
        }
    }
}
