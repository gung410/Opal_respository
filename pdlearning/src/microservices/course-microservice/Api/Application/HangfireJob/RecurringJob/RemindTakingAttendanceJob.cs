using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.Commands;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public class RemindTakingAttendanceJob : BaseHangfireJob, IRemindTakingAttendanceJob
    {
        public RemindTakingAttendanceJob(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
        }

        protected override async Task InternalHandleAsync()
        {
            await ThunderCqrs.SendCommand(new RemindTakeAttendanceCommand() { ForSessionEndTimeAfter = Clock.Now.AddMinutes(30), ForSessionEndTimeBefore = Clock.Now.AddMinutes(45) });
        }
    }
}
