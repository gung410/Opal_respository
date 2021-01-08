using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.BrokenLink.Infrastructure
{
    public class BrokenLinkDbContextResolver : BaseDbContextResolver
    {
        public BrokenLinkDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
