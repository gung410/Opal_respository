using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure
{
    public class LearnerDbContextResolver : BaseDbContextResolver
    {
        public LearnerDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
