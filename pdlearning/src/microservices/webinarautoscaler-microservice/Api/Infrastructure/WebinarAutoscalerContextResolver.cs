using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.WebinarAutoscaler.Infrastructure
{
    public class WebinarAutoscalerContextResolver : BaseDbContextResolver
    {
        public WebinarAutoscalerContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
