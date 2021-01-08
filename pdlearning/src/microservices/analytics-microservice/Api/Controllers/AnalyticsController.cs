using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Analytics.Controllers
{
    [Route("api/analytics")]
    public class AnalyticsController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public AnalyticsController(
            IUserContext userContext,
            IThunderCqrs thunderCqrs) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }
    }
}
