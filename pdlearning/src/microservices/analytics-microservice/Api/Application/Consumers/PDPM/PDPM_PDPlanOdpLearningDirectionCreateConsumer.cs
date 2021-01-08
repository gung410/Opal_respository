using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.PDPM.Messages;
using Microservice.Analytics.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.PDPM
{
    [OpalConsumer("cx-competence-api.pdplan.create.learningdirection")]
    public class PDPM_PDPlanOdpLearningDirectionCreateConsumer : ScopedOpalMessageConsumer<PDPM_PDPlanCreateMessage>
    {
        private readonly IRepository<PDPM_ODP_LearningDirectionStatusHistory> _pdpmOdpLearningDirectionHistoryRepo;

        public PDPM_PDPlanOdpLearningDirectionCreateConsumer(
            IRepository<PDPM_ODP_LearningDirectionStatusHistory> pdpmOdpLearningDirectionHistoryRepository)
        {
            _pdpmOdpLearningDirectionHistoryRepo = pdpmOdpLearningDirectionHistoryRepository;
        }

        public async Task InternalHandleAsync(PDPM_PDPlanCreateMessage message)
        {
            var now = Clock.Now;

            var histories = await _pdpmOdpLearningDirectionHistoryRepo.GetAllListAsync(t => t.ODP_LearningDirectionId == message.Result.ResultIdentity.ExtId);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpLearningDirectionHistoryRepo.UpdateManyAsync(histories);

            await _pdpmOdpLearningDirectionHistoryRepo.InsertAsync(new PDPM_ODP_LearningDirectionStatusHistory()
            {
                ODP_LearningDirectionId = message.Result.ResultIdentity.ExtId,
                ResultId = message.Result.ResultIdentity.Id,
                StatusTypeId = message.Result.AssessmentStatusInfo.AssessmentStatusId,
                ByUserId = message.Result.LastUpdatedBy.Identity.ExtId,
                ValidFrom = now
            });
        }
    }
}
