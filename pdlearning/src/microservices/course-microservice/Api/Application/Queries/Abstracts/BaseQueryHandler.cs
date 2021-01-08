using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Validation;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Queries.Abstracts
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult> where TQuery : BaseThunderQuery<TResult>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected BaseQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager)
        {
            UserContext = userContext;
            AccessControlContext = accessControlContext;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected IUserContext UserContext { get; }

        protected IAccessControlContext<CourseUser> AccessControlContext { get; }

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

        protected List<string> CurrentUserRoles
        {
            get
            {
                var userRoles = UserContext.GetValue<List<string>>(CommonUserContextKeys.UserRoles);
                return userRoles ?? new List<string>();
            }
        }

        public override async Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                EnsureBusinessLogicValid(await ValidateQuery(request, cancellationToken));
                return await HandleAsync(request, cancellationToken);
            }
        }

        protected void EnsureValidPermission(bool checkHasPermission)
        {
            if (!checkHasPermission)
            {
                throw new BusinessLogicException(ExceptionMsgsConstant.InvalidPermission);
            }
        }

        protected async Task EnsureValidPermission(Task<bool> checkHasPermission)
        {
            var hasPermission = await checkHasPermission;
            EnsureValidPermission(hasPermission);
        }

        protected void EnsureBusinessLogicValid(Validation queryValidation)
        {
            if (!queryValidation.IsValid)
            {
                throw new BusinessLogicException(queryValidation.ErrorsMsg());
            }
        }

        protected async Task EnsureBusinessLogicValidAsync(Func<Task<Validation>> validateFunc)
        {
            var validationResult = await validateFunc();
            EnsureBusinessLogicValid(validationResult);
        }

        [return: NotNull]
        [JetBrains.Annotations.NotNull]
        protected TEntity EnsureEntityFound<TEntity>([MaybeNull]TEntity entity)
        {
            if (entity == null)
            {
                throw new EntityNotFoundException();
            }

            return entity;
        }

        protected virtual Task<Validation<TQuery>> ValidateQuery(TQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Validation.Valid(query));
        }

        protected bool HasPermission(params string[] permissionActionKeys)
        {
            return UserContext.HasPermission(permissionActionKeys);
        }

        protected bool HasPermissionPrefix(params string[] permissionActionKeys)
        {
            return UserContext.HasPermissionPrefix(permissionActionKeys);
        }
    }
}
