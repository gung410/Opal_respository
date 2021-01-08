using Microservice.Form.Application.Services;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Controllers
{
    public abstract class BaseController<TApplicationService> : ApplicationApiController where TApplicationService : BaseApplicationService
    {
        protected BaseController(IUserContext userContext, TApplicationService appService) : base(userContext)
        {
            AppService = appService;
        }

        protected TApplicationService AppService { get; }
    }
}
