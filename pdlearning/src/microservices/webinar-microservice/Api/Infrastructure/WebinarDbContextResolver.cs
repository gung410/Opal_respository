using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Webinar.Infrastructure
{
    public class WebinarDbContextResolver : BaseDbContextResolver
    {
        public WebinarDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
