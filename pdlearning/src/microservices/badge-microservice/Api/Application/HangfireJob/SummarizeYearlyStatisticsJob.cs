using System;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Badge.Infrastructure;

namespace Microservice.Badge.Application.HangfireJob
{
    public class SummarizeYearlyStatisticsJob : BaseHangfireJob, ISummarizeMonthlyStatisticsForBadging
    {
        public SummarizeYearlyStatisticsJob(BadgeDbContext badgeDbContext) : base(badgeDbContext)
        {
        }

        public Task ExecuteTask(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
