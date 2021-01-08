using System;
using System.Collections.Generic;
using Conexus.Opal.AccessControl.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult> where TQuery : BaseStandaloneSurveyQuery<TResult>
    {
        protected BaseQueryHandler(IAccessControlContext accessControlContext)
        {
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
    }
}
