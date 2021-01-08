using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.PDPM.Messages;
using Microservice.Analytics.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.PDPM
{
    [OpalConsumer("cx-competence-api.pdplan.create.actionitem")]
    public class PDPM_PDPlanIdpActionItemCreateConsumer : ScopedOpalMessageConsumer<PDPM_PDPlanCreateMessage>
    {
        private readonly IRepository<PDPM_IDP_PDO_StatusHistory> _pdpmOdpLearningDirectionHistoryRepo;

        public PDPM_PDPlanIdpActionItemCreateConsumer(
            IRepository<PDPM_IDP_PDO_StatusHistory> pdpmOdpLearningDirectionHistoryRepository)
        {
            _pdpmOdpLearningDirectionHistoryRepo = pdpmOdpLearningDirectionHistoryRepository;
        }

        public async Task InternalHandleAsync(PDPM_PDPlanCreateMessage message)
        {
            var now = Clock.Now;

            var histories = await _pdpmOdpLearningDirectionHistoryRepo.GetAllListAsync(t => t.IDP_PDO_Id == message.Result.ResultIdentity.ExtId);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpLearningDirectionHistoryRepo.UpdateManyAsync(histories);

            await _pdpmOdpLearningDirectionHistoryRepo.InsertAsync(new PDPM_IDP_PDO_StatusHistory()
            {
                IDP_PDO_Id = message.Result.ResultIdentity.ExtId,
                StatusTypeId = message.Result.AssessmentStatusInfo.AssessmentStatusId,
                ByUserId = message.Result.LastUpdatedBy.Identity.ExtId,
                ValidFrom = now,
                ResultId = message.Result.ResultIdentity.Id
            });
        }
    }
}
