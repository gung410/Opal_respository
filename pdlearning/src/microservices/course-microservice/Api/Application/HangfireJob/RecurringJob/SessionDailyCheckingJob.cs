using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Events.WebinarEvents.WebinarMeetingEvent;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    /// <summary>
    /// Use ICourseDailyCheckingJob temporarily because cannot create ISessionDailyCheckingJob at this time.
    /// </summary>
    public class SessionDailyCheckingJob : BaseHangfireJob, ICourseDailyCheckingJob
    {
        private readonly OpalSettingsOption _opalSettingsOption;
        private readonly BookWebinarMeetingLogic _bookingWebinarMeetingLogic;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public SessionDailyCheckingJob(
            IThunderCqrs thunderCqrs,
            IOptions<OpalSettingsOption> opalSettingsOption,
            IReadOnlyRepository<Session> readSessionRepository,
            BookWebinarMeetingLogic bookingWebinarMeetingLogic,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _opalSettingsOption = opalSettingsOption.Value;
            _bookingWebinarMeetingLogic = bookingWebinarMeetingLogic;
            _readSessionRepository = readSessionRepository;
        }

        public static async Task HandleAsync(
            IReadOnlyRepository<Session> readSessionRepository,
            OpalSettingsOption opalSettingsOption,
            BookWebinarMeetingLogic bookingWebinarMeetingLogic)
        {
            var canBookMeetingSessionsQuery = readSessionRepository.GetAll()
                .Where(Session.CanBookMeetingExpr(opalSettingsOption.MaxMinutesCanJoinWebinarEarly + opalSettingsOption.CreateWebinarDelayTimeInMinutes));

            await bookingWebinarMeetingLogic.BookMeeting(canBookMeetingSessionsQuery, WebinarMeetingAction.Update);
        }

        protected override async Task InternalHandleAsync()
        {
            await HandleAsync(
                _readSessionRepository,
                _opalSettingsOption,
                _bookingWebinarMeetingLogic);
        }
    }
}
