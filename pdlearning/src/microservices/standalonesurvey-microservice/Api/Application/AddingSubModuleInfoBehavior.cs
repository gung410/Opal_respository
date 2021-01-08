using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Queries;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Application
{
    public class AddingSubModuleInfoBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IUserContext _userContext;

        public AddingSubModuleInfoBehavior(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is BaseStandaloneSurveyQuery<TResponse> query)
            {
                query.SubModule = _userContext.GetValue<SubModule>(nameof(IHasSubModuleInfo.SubModule));
                query.CommunityId = _userContext.GetValue<Guid>(nameof(IHasSubModuleInfo.CommunityId));
            }
            else if (request is BaseStandaloneSurveyCommand command)
            {
                command.SubModule = _userContext.GetValue<SubModule>(nameof(IHasSubModuleInfo.SubModule));
                command.CommunityId = _userContext.GetValue<Guid>(nameof(IHasSubModuleInfo.CommunityId));
            }

            return await next();
        }
    }
}
