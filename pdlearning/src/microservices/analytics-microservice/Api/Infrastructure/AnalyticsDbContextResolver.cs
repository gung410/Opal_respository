using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure
{
    public class AnalyticsDbContextResolver : BaseDbContextResolver
    {
        public AnalyticsDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
