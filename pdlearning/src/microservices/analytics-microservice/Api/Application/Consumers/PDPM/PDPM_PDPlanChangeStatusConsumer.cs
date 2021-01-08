using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.PDPM.Messages;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.PDPM
{
    [OpalConsumer("cx-competence-api.pdplan.change.status_type")]
    public class PDPM_PDPlanChangeStatusConsumer : ScopedOpalMessageConsumer<PDPM_PDPlanChangeStatusMessage>
    {
        private readonly Func<AnalyticPdPlanActivity, IAnalyticsPdpmPdPlanStatusChangeService> _analyticsPdpmPdPlanStatusChangeServiceFunc;

        public PDPM_PDPlanChangeStatusConsumer(
            Func<AnalyticPdPlanActivity, IAnalyticsPdpmPdPlanStatusChangeService> analyticsPdpmPdPlanStatusChangeServiceFunc)
        {
            _analyticsPdpmPdPlanStatusChangeServiceFunc = analyticsPdpmPdPlanStatusChangeServiceFunc;
        }

        public async Task InternalHandleAsync(PDPM_PDPlanChangeStatusMessage message)
        {
            // we just handle:
            // - idp: learning need, action item
            // - odp: learning plan, direction, programme
            // so we don't need to check the pdPlanType
            var analyticsPdpmPdPlanStatusChangeService = _analyticsPdpmPdPlanStatusChangeServiceFunc(message.Result.PdPlanActivity);
            await analyticsPdpmPdPlanStatusChangeService.CreateHistoryItem(
                message.Result.ResultExtId,
                message.Result.TargetResultId,
                message.Result.TargetStatusType.StatusTypeId,
                message.AdditionalInformation.UpdatedBy);
        }
    }
}
