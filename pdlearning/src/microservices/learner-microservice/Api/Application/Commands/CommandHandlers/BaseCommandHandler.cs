using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public abstract class BaseCommandHandler<TCommand> : BaseThunderCommandHandler<TCommand> where TCommand : BaseThunderCommand
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected BaseCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext)
        {
            UserContext = userContext;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected IUserContext UserContext { get; }

        protected Guid? CurrentUserId
        {
            get
            {
                var userId = UserContext.GetValue<string>(CommonUserContextKeys.UserId);
                return !string.IsNullOrEmpty(userId) ? (Guid?)Guid.Parse(userId) : null;
            }
        }

        protected Guid CurrentUserIdOrDefault
        {
            get
            {
                return CurrentUserId ?? Guid.Empty;
            }
        }

        protected override async Task Handle(TCommand request, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await HandleAsync(request, cancellationToken);
                await uow.CompleteAsync();
            }
        }
    }
}
