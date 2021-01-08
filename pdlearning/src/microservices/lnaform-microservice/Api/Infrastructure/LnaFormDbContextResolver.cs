using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.LnaForm.Infrastructure
{
    public class LnaFormDbContextResolver : BaseDbContextResolver
    {
        public LnaFormDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
