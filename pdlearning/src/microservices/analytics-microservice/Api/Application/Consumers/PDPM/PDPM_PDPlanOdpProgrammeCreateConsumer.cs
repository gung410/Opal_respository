using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.PDPM.Messages;
using Microservice.Analytics.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.PDPM
{
    [OpalConsumer("cx-competence-api.pdplan.create.learningprogramme")]
    public class PDPM_PDPlanOdpProgrammeCreateConsumer : ScopedOpalMessageConsumer<PDPM_PDPlanCreateMessage>
    {
        private readonly IRepository<PDPM_ODP_KLP_StatusHistory> _pdpmOdpKlpStatusHistoryRepo;

        public PDPM_PDPlanOdpProgrammeCreateConsumer(
            IRepository<PDPM_ODP_KLP_StatusHistory> pdpmOdpKlpStatusHistoryRepo)
        {
            _pdpmOdpKlpStatusHistoryRepo = pdpmOdpKlpStatusHistoryRepo;
        }

        public async Task InternalHandleAsync(PDPM_PDPlanCreateMessage message)
        {
            var now = Clock.Now;

            var histories = await _pdpmOdpKlpStatusHistoryRepo.GetAllListAsync(t => t.ODP_KLP_Id == message.Result.ResultIdentity.ExtId);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpKlpStatusHistoryRepo.UpdateManyAsync(histories);

            await _pdpmOdpKlpStatusHistoryRepo.InsertAsync(new PDPM_ODP_KLP_StatusHistory()
            {
                ODP_KLP_Id = message.Result.ResultIdentity.ExtId,
                ResultId = message.Result.ResultIdentity.Id,
                StatusTypeId = message.Result.AssessmentStatusInfo.AssessmentStatusId,
                ByUserId = message.Result.LastUpdatedBy.Identity.ExtId,
                ValidFrom = now
            });
        }
    }
}
