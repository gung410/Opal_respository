using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public class StandaloneSurveyDbContextResolver : BaseDbContextResolver
    {
        public StandaloneSurveyDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
