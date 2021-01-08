using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.PDPM.Messages;
using Microservice.Analytics.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.PDPM
{
    [OpalConsumer("cx-competence-api.pdplan.create.learningneed")]
    public class PDPM_PDPlanIdpLearningNeedCreateConsumer : ScopedOpalMessageConsumer<PDPM_PDPlanCreateMessage>
    {
        private readonly IRepository<PDPM_IDP_LNA_StatusHistory> _pdpmIdpLnaStatusHistoryRepo;

        public PDPM_PDPlanIdpLearningNeedCreateConsumer(
            IRepository<PDPM_IDP_LNA_StatusHistory> pdpmIdpLnaStatusHistoryRepo)
        {
            _pdpmIdpLnaStatusHistoryRepo = pdpmIdpLnaStatusHistoryRepo;
        }

        public async Task InternalHandleAsync(PDPM_PDPlanCreateMessage message)
        {
            var now = Clock.Now;

            var histories = await _pdpmIdpLnaStatusHistoryRepo.GetAllListAsync(t => t.IDP_LNA_Id == message.Result.ResultIdentity.ExtId);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmIdpLnaStatusHistoryRepo.UpdateManyAsync(histories);

            await _pdpmIdpLnaStatusHistoryRepo.InsertAsync(new PDPM_IDP_LNA_StatusHistory()
            {
                IDP_LNA_Id = message.Result.ResultIdentity.ExtId,
                ResultId = message.Result.ResultIdentity.Id,
                StatusTypeId = message.Result.AssessmentStatusInfo.AssessmentStatusId,
                ByUserId = message.Result.LastUpdatedBy.Identity.ExtId,
                ValidFrom = now
            });
        }
    }
}
