using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Calendar.Infrastructure
{
    public class CalendarDbContextResolver : BaseDbContextResolver
    {
        public CalendarDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
