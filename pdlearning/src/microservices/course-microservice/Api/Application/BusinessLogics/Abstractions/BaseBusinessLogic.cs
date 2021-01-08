using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microservice.Course.Application.Constants;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.BusinessLogics.Abstractions
{
    public abstract class BaseBusinessLogic : IBusinessLogic
    {
        private readonly IUserContext _userContext;

        protected BaseBusinessLogic(IUserContext userContext)
        {
            _userContext = userContext;
        }

        protected Guid? CurrentUserId
        {
            get
            {
                var userId = _userContext.GetValue<string>(CommonUserContextKeys.UserId);
                return !string.IsNullOrEmpty(userId) ? (Guid?)Guid.Parse(userId) : null;
            }
        }

        protected List<string> CurrentUserRoles
        {
            get
            {
                var userRoles = _userContext.GetValue<List<string>>(CommonUserContextKeys.UserRoles);
                return userRoles ?? new List<string>();
            }
        }

        protected Guid CurrentUserIdOrDefault => CurrentUserId ?? Guid.Empty;

        protected T EnsureBusinessLogicValid<T>(Validation<T> validateResult)
        {
            if (!validateResult.IsValid)
            {
                throw new BusinessLogicException(validateResult.ErrorsMsg());
            }

            return validateResult.Target;
        }

        protected void EnsureBusinessLogicValid(Validation queryValidation)
        {
            if (!queryValidation.IsValid)
            {
                throw new BusinessLogicException(queryValidation.ErrorsMsg());
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

        protected bool HasPermissionPrefix(params string[] permissionActionKeys)
        {
            return _userContext.HasPermissionPrefix(permissionActionKeys);
        }

        protected void EnsureValidPermission(bool checkHasPermission)
        {
            if (!checkHasPermission)
            {
                throw new BusinessLogicException(ExceptionMsgsConstant.InvalidPermission);
            }
        }
    }
}
