using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public abstract class BaseCommandHandler<TCommand> : BaseThunderCommandHandler<TCommand> where TCommand : BaseThunderCommand
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected BaseCommandHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
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
