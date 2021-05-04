using System;
using cxOrganization.Adapter.Assessment;
using cxPlatform.Core;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.Domain.Extensions;
using cxPlatform.Core.Cache;
using Microsoft.Extensions.Logging;
using cxOrganization.Domain.AdvancedWorkContext;

namespace cxOrganization.Business.PDPlanner
{
    public class LearningNeedsAnalysisService : ILearningNeedsAnalysisService
    {
        private readonly IAdvancedWorkContext _workContext;
        private readonly IAssessmentAdapter _assessmentAdapter;
        private readonly LearningNeedsAnalysisConfig _learningNeedsAnalysisConfig;
        private readonly IAsyncDistributedCacheProvider _asyncDistributedCacheProvider;
        private readonly ILogger _logger;
        public LearningNeedsAnalysisService(ILogger<LearningNeedsAnalysisService> logger, IAdvancedWorkContext workContext,
            IAssessmentAdapter assessmentAdapter,
            IOptions<LearningNeedsAnalysisConfig> learningNeedsAnalysisConfig,
            IAsyncDistributedCacheProvider asyncDistributedCacheProvider)
        {
            _workContext = workContext;
            _assessmentAdapter = assessmentAdapter;
            _learningNeedsAnalysisConfig = learningNeedsAnalysisConfig?.Value;
            _asyncDistributedCacheProvider = asyncDistributedCacheProvider;
            _logger = logger;
        }

        /// <summary>
        /// Gets the learning area priorities from the latest published Learning Needs Analysis.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="customerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<LearningAreaPriority> GetLearningAreaPrioritiesFromLatestPublishedAnswer(
            int ownerId, int customerId,
            int userId
        )
        {
            return await _assessmentAdapter
                .GetLearningAreaPrioritiesFromLatestPublishedAnswer(ownerId, customerId, userId,
                    _learningNeedsAnalysisConfig.ActivityExtId,
                    _learningNeedsAnalysisConfig.StatusTypeCodeNamesUsableForReport,
                    _learningNeedsAnalysisConfig.JsonAnswerLearningAreaPriorityPropertyPrefix);
        }

        public async Task<(List<string> AllTagIds, LearningAreaPriority LearningAreaPriority)>
            GetLearningAreaPriorityTagIdsFromLNAData(int userId,
                bool includeLearningAreaTagsMarkedHighPriority,
                bool includeLearningAreaTagsMarkedModeratePriority,
                bool includeLearningAreaTagsMarkedLowPriority)
        {
            var learningAreaPriorityTagIds = new List<string>();
            var cacheKey = BuildLearningAreaPriorityCacheKey(userId);
            var learningAreaPriority = _learningNeedsAnalysisConfig.CacheLearningAreaPriority
                ? await _asyncDistributedCacheProvider.GetAsync<LearningAreaPriority>(cacheKey)
                : null;
            if (learningAreaPriority == null)
            {

                learningAreaPriority = await _assessmentAdapter
                    .GetLearningAreaPrioritiesFromLatestPublishedAnswer(_workContext.CurrentOwnerId,
                        _workContext.CurrentCustomerId, userId, _learningNeedsAnalysisConfig.ActivityExtId,
                        _learningNeedsAnalysisConfig.StatusTypeCodeNamesUsableForReport,

                        _learningNeedsAnalysisConfig.JsonAnswerLearningAreaPriorityPropertyPrefix);
           
                if (_learningNeedsAnalysisConfig.CacheLearningAreaPriority && learningAreaPriority != null)
                {
                    await _asyncDistributedCacheProvider.AddAsync(cacheKey, learningAreaPriority,
                        _learningNeedsAnalysisConfig.CacheDuration ?? new TimeSpan());
                }
              
            }
            else
            {
                _logger.LogDebug($"LearningAreaPriority of user {userId} is retrieved from distributed cache");
            }

            if (learningAreaPriority == null)
            {
                return (learningAreaPriorityTagIds, null);
            }

            if (includeLearningAreaTagsMarkedHighPriority && !learningAreaPriority.HighPriorities.IsNullOrEmpty())
            {
                learningAreaPriorityTagIds.AddRange(learningAreaPriority.HighPriorities);
            }

            if (includeLearningAreaTagsMarkedModeratePriority && !learningAreaPriority.ModeratePriorities.IsNullOrEmpty())
            {
                learningAreaPriorityTagIds.AddRange(learningAreaPriority.ModeratePriorities);
            }

            if (includeLearningAreaTagsMarkedLowPriority && !learningAreaPriority.LowPriorities.IsNullOrEmpty())
            {
                learningAreaPriorityTagIds.AddRange(learningAreaPriority.LowPriorities);

            }

            return (learningAreaPriorityTagIds, learningAreaPriority);
        }

        public async Task<Dictionary<int,bool>> ClearLearningAreaPriorityFromCache(List<int> userIds)
        {
            if (!_learningNeedsAnalysisConfig.CacheLearningAreaPriority || userIds.IsNullOrEmpty()) return new Dictionary<int, bool>();
        
            var clearCacheResults = new Dictionary<int,bool>();

            foreach (var userId in userIds)
            {
                var cacheKey = BuildLearningAreaPriorityCacheKey(userId);
                var clearCacheSuccessfully = await _asyncDistributedCacheProvider.RemoveAsync(cacheKey);


                if (clearCacheSuccessfully)
                {
                    _logger.LogDebug($"LearningAreaPriority of user {userId} is remove from distributed cache");
                }
                else
                {
                    _logger.LogDebug($"Failed to remove LearningAreaPriority of user {userId} from distributed cache");
                }

                clearCacheResults.Add(userId, clearCacheSuccessfully);

            }

            return clearCacheResults;
        }
        private string BuildLearningAreaPriorityCacheKey(int userId)
        {
            var cacheKeyBuilder = new StringBuilder("LearningAreaPriority#")
                .Append($"_{_workContext.CurrentOwnerId}")
                .Append($"_{_workContext.CurrentCustomerId}")
                .Append($"_{userId}")
                .Append($"_{_learningNeedsAnalysisConfig.ActivityExtId}")
                .Append($"_{string.Join(",", _learningNeedsAnalysisConfig.StatusTypeCodeNamesUsableForReport)}")
                .Append($"_{_learningNeedsAnalysisConfig.JsonAnswerLearningAreaPriorityPropertyPrefix}");

            return cacheKeyBuilder.ToString();
        }


    }
}
