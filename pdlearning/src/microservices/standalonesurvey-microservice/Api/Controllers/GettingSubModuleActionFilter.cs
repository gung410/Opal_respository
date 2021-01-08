using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Controllers
{
    public sealed class GettingSubModuleActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "GET")
            {
                ProcessForGetMethod(context);
            }
            else
            {
                ProcessForOtherMethods(context);
            }

            base.OnActionExecuting(context);
        }

        private void ProcessForOtherMethods(ActionExecutingContext context)
        {
            foreach (var actionArgumentsValue in context.ActionArguments.Values)
            {
                if (actionArgumentsValue is IHasSubModuleInfo subModuleInfo)
                {
                    var userContext = context.HttpContext.RequestServices.GetService<IUserContext>();
                    var defaultModule = SubModule.Lna;
                    if (subModuleInfo.SubModule == SubModule.Csl)
                    {
                        userContext.SetValue(subModuleInfo.CommunityId, nameof(IHasSubModuleInfo.CommunityId));
                    }

                    userContext.SetValue(subModuleInfo.SubModule == SubModule.None ? defaultModule : subModuleInfo.SubModule, "SubModule");
                }
            }
        }

        private void ProcessForGetMethod(ActionExecutingContext context)
        {
            SubModule subModule = SubModule.Lna;

            if (context.HttpContext.Request.Query.TryGetValue("subModule", out var module))
            {
                switch (module.ToArray()[0])
                {
                    case "csl":
                        subModule = SubModule.Csl;
                        break;
                    case "lna":
                        subModule = SubModule.Lna;
                        break;
                }
            }

            var userContext = context.HttpContext.RequestServices.GetService<IUserContext>();
            userContext.SetValue(subModule, nameof(IHasSubModuleInfo.SubModule));

            if (subModule == SubModule.Csl)
            {
                context.HttpContext.Request.Query.TryGetValue("subModule", out var communityId);
                userContext.SetValue(communityId, nameof(IHasSubModuleInfo.CommunityId));
            }
        }
    }
}
