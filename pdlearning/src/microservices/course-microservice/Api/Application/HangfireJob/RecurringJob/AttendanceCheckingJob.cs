using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.Commands;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public class AttendanceCheckingJob : BaseHangfireJob, IAttendanceCheckingJob
    {
        public AttendanceCheckingJob(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
        }

        protected override async Task InternalHandleAsync()
        {
            await ThunderCqrs.SendCommand(new InitAttendanceTrackingForSessionCommand { ForDailyTracking = true });
            await ThunderCqrs.SendCommand(new SetAbsentForMissingInfoAttendanceTrackingCommand
            {
                ForSessionStartAfter = Clock.Now.AddDays(-1),
                ForSessionStartBefore = Clock.Now
            });
        }
    }
}
