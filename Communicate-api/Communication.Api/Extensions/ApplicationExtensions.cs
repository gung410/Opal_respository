using cx.datahub.scheduling.jobs.shared;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseRecurringJobs(
            this IApplicationBuilder services, IRecurringJobManager recurringJobManager)
        {
            TimeZoneInfo timeZoneInfo;
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Singapore");
            }
            catch (TimeZoneNotFoundException)
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            }
            recurringJobManager.AddOrUpdate<IDigestEmailJob>("IDigestEmailJob",  job =>  job.ExecuteTask(null),
                "0 8 * * *", timeZoneInfo, queue: "communication_api");
        }
    }
}
