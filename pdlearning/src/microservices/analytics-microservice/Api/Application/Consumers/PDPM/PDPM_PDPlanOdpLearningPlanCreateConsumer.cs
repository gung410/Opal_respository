using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.PDPM.Messages;
using Microservice.Analytics.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.PDPM
{
    [OpalConsumer("cx-competence-api.pdplan.create.learningplan")]
    public class PDPM_PDPlanOdpLearningPlanCreateConsumer : ScopedOpalMessageConsumer<PDPM_PDPlanCreateMessage>
    {
        private readonly IRepository<PDPM_ODP_LearningPlanStatusHistory> _pdpmOdpLearningPlanStatusHistoryRepo;

        public PDPM_PDPlanOdpLearningPlanCreateConsumer(
            IRepository<PDPM_ODP_LearningPlanStatusHistory> pdpmOdpLearningPlanStatusHistoryRepo)
        {
            _pdpmOdpLearningPlanStatusHistoryRepo = pdpmOdpLearningPlanStatusHistoryRepo;
        }

        public async Task InternalHandleAsync(PDPM_PDPlanCreateMessage message)
        {
            if (message.Result.PdPlanType.Equals("Odp", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var now = Clock.Now;

            var histories = await _pdpmOdpLearningPlanStatusHistoryRepo.GetAllListAsync(t => t.ODP_LearningPlanId == message.Result.ResultIdentity.ExtId);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpLearningPlanStatusHistoryRepo.UpdateManyAsync(histories);

            await _pdpmOdpLearningPlanStatusHistoryRepo.InsertAsync(new PDPM_ODP_LearningPlanStatusHistory()
            {
                ODP_LearningPlanId = message.Result.ResultIdentity.ExtId,
                ResultId = message.Result.ResultIdentity.Id,
                StatusTypeId = message.Result.AssessmentStatusInfo.AssessmentStatusId,
                ByUserId = message.Result.LastUpdatedBy.Identity.ExtId,
                ValidFrom = now
            });
        }
    }
}
