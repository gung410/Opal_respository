using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Constants;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Migrations;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Validation;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Commands.Abstracts
{
    public abstract class BaseCommandHandler<TCommand> : BaseThunderCommandHandler<TCommand> where TCommand : BaseThunderCommand
    {
        private readonly IUserContext _userContext;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        protected BaseCommandHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userContext = userContext;
            _unitOfWorkManager = unitOfWorkManager;
            AccessControlContext = accessControlContext;
        }

        protected IAccessControlContext<CourseUser> AccessControlContext { get; }

        protected Guid? CurrentUserId
        {
            get
            {
                var userId = _userContext.GetValue<string>(CommonUserContextKeys.UserId);
                return !string.IsNullOrEmpty(userId) ? (Guid?)Guid.Parse(userId) : null;
            }
        }

        protected Guid CurrentUserIdOrDefault => CurrentUserId ?? Guid.Empty;

        protected List<string> CurrentUserRoles
        {
            get
            {
                var userRoles = _userContext.GetValue<List<string>>(CommonUserContextKeys.UserRoles);
                return userRoles ?? new List<string>();
            }
        }

        protected override async Task Handle(TCommand request, CancellationToken cancellationToken)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                EnsureBusinessLogicValid(await ValidateCommand(request));
                await HandleAsync(request, cancellationToken);
                await uow.CompleteAsync();
            }
        }

        protected void EnsureValidPermission(bool checkHasPermission)
        {
            if (!checkHasPermission)
            {
                throw new BusinessLogicException(ExceptionMsgsConstant.InvalidPermission);
            }
        }

        protected void EnsureValidPermission<T>(IEnumerable<T> items, Func<T, bool> checkHasPermissionFn)
        {
            if (items.Any(p => !checkHasPermissionFn(p)))
            {
                throw new BusinessLogicException(ExceptionMsgsConstant.InvalidPermission);
            }
        }

        protected async Task EnsureValidPermission(Task<bool> checkHasPermission)
        {
            var hasPermission = await checkHasPermission;
            EnsureValidPermission(hasPermission);
        }

        protected T EnsureBusinessLogicValid<T>(Validation<T> validateResult)
        {
            if (!validateResult.IsValid)
            {
                throw new BusinessLogicException(validateResult.ErrorsMsg());
            }

            return validateResult.Target;
        }

        protected void EnsureBusinessLogicValid(params Validation[] validateResults)
        {
            var finalValidationResult = Validation.HarvestErrors(validateResults);
            if (!finalValidationResult.IsValid)
            {
                throw new BusinessLogicException(finalValidationResult.ErrorsMsg());
            }
        }

        protected void EnsureBusinessLogicValid<TEntity>([NotNull] List<TEntity> entities, Func<TEntity, Validation> validationFunc)
        {
            var validationResult = Validation.FailFast(entities.Select(validationFunc).ToArray());
            if (!validationResult.IsValid)
            {
                throw new BusinessLogicException(validationResult.ErrorsMsg());
            }
        }

        protected void EnsureBusinessLogicValid<TEntity>([NotNull] List<TEntity> entities, Func<TEntity, bool> isValidFunc)
        {
            if (entities.Any(p => !isValidFunc(p)))
            {
                throw new BusinessLogicException(ExceptionMsgsConstant.InvalidData);
            }
        }

        protected void EnsureBusinessLogicValid<TEntity>([NotNull] TEntity entity, Func<TEntity, Validation> validationFunc)
        {
            EnsureBusinessLogicValid(new List<TEntity>() { entity }, validationFunc);
        }

        protected void EnsureBusinessLogicValid<TEntity>([NotNull] TEntity entity, Func<TEntity, bool> isValidFunc)
        {
            EnsureBusinessLogicValid(new List<TEntity>() { entity }, isValidFunc);
        }

        [return: NotNull]
        [JetBrains.Annotations.NotNull]
        protected TEntity EnsureEntityFound<TEntity>([MaybeNull] TEntity entity)
        {
            if (entity == null)
            {
                throw new EntityNotFoundException();
            }

            return entity;
        }

        protected void EnsureEntitiesFound<TEntity>(List<TEntity> entities)
        {
            if (!entities.Any() || entities.All(p => p == null))
            {
                throw new EntityNotFoundException();
            }
        }

        protected void EnsureEntityFound<TEntity>(TEntity entity, Guid entityId)
        {
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), entityId);
            }
        }

        protected virtual Task<Validation<TCommand>> ValidateCommand(TCommand command)
        {
            return Task.FromResult(Validation.Valid(command));
        }

        protected bool HasPermission(params string[] permissionActionKeys)
        {
            return _userContext.HasPermission(permissionActionKeys);
        }

        protected bool HasPermissionPrefix(params string[] permissionActionKeys)
        {
            return _userContext.HasPermissionPrefix(permissionActionKeys);
        }
    }
}
