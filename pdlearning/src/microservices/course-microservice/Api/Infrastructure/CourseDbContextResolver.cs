using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Course.Infrastructure
{
    public class CourseDbContextResolver : BaseDbContextResolver
    {
        public CourseDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
