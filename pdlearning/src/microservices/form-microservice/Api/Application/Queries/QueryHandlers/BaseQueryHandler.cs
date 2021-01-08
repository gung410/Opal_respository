using System;
using Conexus.Opal.AccessControl.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult> where TQuery : BaseThunderQuery<TResult>
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
    }
}
