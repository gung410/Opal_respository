using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public class OfferExpirationCheckingJob : BaseHangfireJob, IOfferExpirationCheckingJob
    {
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly OpalSettingsOption _opalSettingsOption;

        public OfferExpirationCheckingJob(
            IThunderCqrs thunderCqrs,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            IOptions<OpalSettingsOption> opalSettingsOption,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _opalSettingsOption = opalSettingsOption.Value;
        }

        protected override async Task InternalHandleAsync()
        {
            var offerLifeTime = _opalSettingsOption.OfferExpireLifeTime;
            var latestExpiredOfferDate = Clock.Now.Date.AddDays(-offerLifeTime);
            var registrations = await _getAggregatedRegistrationSharedQuery.WithExpiredOffer(latestExpiredOfferDate);
            if (registrations.Any())
            {
                await ThunderCqrs.SendCommand(
                    new ChangeRegistrationStatusCommand()
                    {
                        Ids = registrations.Select(p => p.Registration.Id).ToList(),
                        Status = RegistrationStatus.OfferExpired
                    });
            }
        }
    }
}
