using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.WebinarVideoConverter.Infrastructure
{
    public class WebinarRecordDbContextResolver : BaseDbContextResolver
    {
        public WebinarRecordDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
