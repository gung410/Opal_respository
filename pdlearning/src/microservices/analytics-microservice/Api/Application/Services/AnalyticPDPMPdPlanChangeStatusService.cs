using System;
using System.Threading.Tasks;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Services
{
    public class AnalyticPdpmPdPlanLearningPlanStatusChangeService : AnalyticsShareService, IAnalyticsPdpmPdPlanStatusChangeService
    {
        private readonly IRepository<PDPM_ODP_LearningPlanStatusHistory> _pdpmOdpLearningPlanStatusHistoryRepo;

        public AnalyticPdpmPdPlanLearningPlanStatusChangeService(IRepository<PDPM_ODP_LearningPlanStatusHistory> pdpmOdpLearningPlanStatusHistoryRepo)
        {
            _pdpmOdpLearningPlanStatusHistoryRepo = pdpmOdpLearningPlanStatusHistoryRepo;
        }

        public async Task CreateHistoryItem(Guid resultExtId, long resultId, int statusId, Guid byUserId)
        {
            var now = Clock.Now;

            var histories = await _pdpmOdpLearningPlanStatusHistoryRepo.GetAllListAsync(t => t.ODP_LearningPlanId == resultExtId && t.ValidTo == null);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpLearningPlanStatusHistoryRepo.UpdateManyAsync(histories);

            await _pdpmOdpLearningPlanStatusHistoryRepo.InsertAsync(new PDPM_ODP_LearningPlanStatusHistory()
            {
                ODP_LearningPlanId = resultExtId,
                ResultId = resultId,
                StatusTypeId = statusId,
                ValidFrom = now,
                ByUserId = byUserId
            });
        }
    }

    public class AnalyticPdpmPdPlanLearningNeedStatusChangeService : AnalyticsShareService, IAnalyticsPdpmPdPlanStatusChangeService
    {
        private readonly IRepository<PDPM_IDP_LNA_StatusHistory> _pdpmIdpLnaStatusHistoryRepo;

        public AnalyticPdpmPdPlanLearningNeedStatusChangeService(IRepository<PDPM_IDP_LNA_StatusHistory> pdpmIdpLnaStatusHistoryRepo)
        {
            _pdpmIdpLnaStatusHistoryRepo = pdpmIdpLnaStatusHistoryRepo;
        }

        public async Task CreateHistoryItem(Guid resultExtId, long resultId, int statusId, Guid byUserId)
        {
            var now = Clock.Now;

            var histories = await _pdpmIdpLnaStatusHistoryRepo.GetAllListAsync(t => t.IDP_LNA_Id == resultExtId && t.ValidTo == null);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmIdpLnaStatusHistoryRepo.UpdateManyAsync(histories);

            await _pdpmIdpLnaStatusHistoryRepo.InsertAsync(new PDPM_IDP_LNA_StatusHistory()
            {
                IDP_LNA_Id = resultExtId,
                ResultId = resultId,
                StatusTypeId = statusId,
                ValidFrom = now,
                ByUserId = byUserId
            });
        }
    }

    public class AnalyticPdpmPdPlanActionItemStatusChangeService : AnalyticsShareService, IAnalyticsPdpmPdPlanStatusChangeService
    {
        private readonly IRepository<PDPM_IDP_PDO_StatusHistory> _pdpmOdpLearningDirectionHistoryRepository;

        public AnalyticPdpmPdPlanActionItemStatusChangeService(IRepository<PDPM_IDP_PDO_StatusHistory> pdpmOdpLearningDirectionHistoryRepository)
        {
            _pdpmOdpLearningDirectionHistoryRepository = pdpmOdpLearningDirectionHistoryRepository;
        }

        public async Task CreateHistoryItem(Guid resultExtId, long resultId, int statusId, Guid byUserId)
        {
            var now = Clock.Now;

            var histories = await _pdpmOdpLearningDirectionHistoryRepository.GetAllListAsync(t => t.IDP_PDO_Id == resultExtId && t.ValidTo == null);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpLearningDirectionHistoryRepository.UpdateManyAsync(histories);

            await _pdpmOdpLearningDirectionHistoryRepository.InsertAsync(new PDPM_IDP_PDO_StatusHistory()
            {
                IDP_PDO_Id = resultExtId,
                ResultId = resultId,
                StatusTypeId = statusId,
                ValidFrom = now,
                ByUserId = byUserId
            });
        }
    }

    public class AnalyticPdpmPdPlanLearningDirectionStatusChangeService : AnalyticsShareService, IAnalyticsPdpmPdPlanStatusChangeService
    {
        private readonly IRepository<PDPM_ODP_LearningDirectionStatusHistory> _pdpmOdpLearningDirectionHistoryRepo;

        public AnalyticPdpmPdPlanLearningDirectionStatusChangeService(IRepository<PDPM_ODP_LearningDirectionStatusHistory> pdpmOdpLearningDirectionHistoryRepo)
        {
            _pdpmOdpLearningDirectionHistoryRepo = pdpmOdpLearningDirectionHistoryRepo;
        }

        public async Task CreateHistoryItem(Guid resultExtId, long resultId, int statusId, Guid byUserId)
        {
            var now = Clock.Now;

            var histories = await _pdpmOdpLearningDirectionHistoryRepo.GetAllListAsync(t => t.ODP_LearningDirectionId == resultExtId && t.ValidTo == null);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpLearningDirectionHistoryRepo.UpdateManyAsync(histories);

            await _pdpmOdpLearningDirectionHistoryRepo.InsertAsync(new PDPM_ODP_LearningDirectionStatusHistory()
            {
                ODP_LearningDirectionId = resultExtId,
                StatusTypeId = statusId,
                ValidFrom = now,
                ResultId = resultId,
                ByUserId = byUserId
            });
        }
    }

    public class AnalyticPdpmPdPlanLearningProgrammeStatusChangeService : AnalyticsShareService, IAnalyticsPdpmPdPlanStatusChangeService
    {
        private readonly IRepository<PDPM_ODP_KLP_StatusHistory> _pdpmOdpKlpStatusHistoryRepo;

        public AnalyticPdpmPdPlanLearningProgrammeStatusChangeService(IRepository<PDPM_ODP_KLP_StatusHistory> pdpmOdpKlpStatusHistoryRepo)
        {
            _pdpmOdpKlpStatusHistoryRepo = pdpmOdpKlpStatusHistoryRepo;
        }

        public async Task CreateHistoryItem(Guid resultExtId, long resultId, int statusId, Guid byUserId)
        {
            var now = Clock.Now;

            var histories = await _pdpmOdpKlpStatusHistoryRepo.GetAllListAsync(t => t.ODP_KLP_Id == resultExtId && t.ValidTo == null);

            foreach (var history in histories)
            {
                history.ValidTo = now;
            }

            await _pdpmOdpKlpStatusHistoryRepo.UpdateManyAsync(histories);

            await _pdpmOdpKlpStatusHistoryRepo.InsertAsync(new PDPM_ODP_KLP_StatusHistory()
            {
                ODP_KLP_Id = resultExtId,
                StatusTypeId = statusId,
                ValidFrom = now,
                ResultId = resultId,
                ByUserId = byUserId
            });
        }
    }
}
