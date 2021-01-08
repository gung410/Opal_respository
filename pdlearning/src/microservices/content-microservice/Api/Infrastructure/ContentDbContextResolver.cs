using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Content.Infrastructure
{
    public class ContentDbContextResolver : BaseDbContextResolver
    {
        public ContentDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
