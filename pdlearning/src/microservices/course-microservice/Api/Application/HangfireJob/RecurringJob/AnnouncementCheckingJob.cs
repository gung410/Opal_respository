using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    /// <summary>
    /// Announcement checking internal 15 minutes.
    /// </summary>
    public class AnnouncementCheckingJob : BaseHangfireJob, IAnnouncementCheckingJob
    {
        public static int CheckingIntervalMinutes = 15;

        public AnnouncementCheckingJob(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
        }

        protected override async Task InternalHandleAsync()
        {
            await ThunderCqrs.SendCommand(
                    new ChangeAnnouncementStatusCommand
                    {
                        Status = AnnouncementStatus.Sent,
                        ForAnnouncements = new SendAnnouncementCommandSearchCondition
                        {
                            ScheduleDateBefore = Clock.Now.AddSeconds(1)
                        }
                    });
        }
    }
}
