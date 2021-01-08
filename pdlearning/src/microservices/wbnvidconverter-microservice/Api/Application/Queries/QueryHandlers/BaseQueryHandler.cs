using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Queries
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult>
        where TQuery : BaseThunderQuery<TResult>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected BaseQueryHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                return await HandleAsync(request, cancellationToken);
            }
        }
    }
}
