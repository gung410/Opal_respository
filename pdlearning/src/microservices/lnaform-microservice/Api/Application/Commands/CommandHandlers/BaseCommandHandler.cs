using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public abstract class BaseCommandHandler<TCommand> : BaseThunderCommandHandler<TCommand> where TCommand : BaseThunderCommand
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected BaseCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext)
        {
            _unitOfWorkManager = unitOfWorkManager;
            AccessControlContext = accessControlContext;
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

        protected List<string> CurrentUserRoles
        {
            get
            {
                var userRoles = AccessControlContext.UserContext.GetValue<List<string>>(CommonUserContextKeys.UserRoles);
                return userRoles ?? new List<string>();
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
