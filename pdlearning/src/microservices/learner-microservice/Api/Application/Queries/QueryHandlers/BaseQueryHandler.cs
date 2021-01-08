using System;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult> where TQuery : BaseThunderQuery<TResult>
    {
        protected BaseQueryHandler(IUserContext userContext)
        {
            UserContext = userContext;
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
    }
}
