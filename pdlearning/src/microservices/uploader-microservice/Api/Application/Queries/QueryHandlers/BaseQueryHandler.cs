using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Queries
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult> where TQuery : BaseThunderQuery<TResult>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected BaseQueryHandler(IAccessControlContext accessControlContext, IUnitOfWorkManager unitOfWorkManager)
        {
            AccessControlContext = accessControlContext;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected IAccessControlContext AccessControlContext { get; }

        protected Guid CurrentUserId
        {
            get
            {
                var userId = AccessControlContext.UserContext.GetValue<string>(CommonUserContextKeys.UserId);
                return !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : Guid.Empty;
            }
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
