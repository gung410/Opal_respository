using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Conexus.Opal.Microservice.Tagging.Infrastructure
{
    public class TaggingDbContextResolver : BaseDbContextResolver
    {
        public TaggingDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
