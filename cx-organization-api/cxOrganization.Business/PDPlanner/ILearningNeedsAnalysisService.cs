using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment;

namespace cxOrganization.Business.PDPlanner
{
    public interface ILearningNeedsAnalysisService
    {
        /// <summary>
        /// Gets the learning area priorities from the latest published Learning Needs Analysis.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="customerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<LearningAreaPriority> GetLearningAreaPrioritiesFromLatestPublishedAnswer(
            int ownerId, int customerId,
            int userId);

        Task<(List<string> AllTagIds, LearningAreaPriority LearningAreaPriority)> GetLearningAreaPriorityTagIdsFromLNAData(int userId,
            bool includeLearningAreaTagsMarkedHighPriority,
            bool includeLearningAreaTagsMarkedModeratePriority,
            bool includeLearningAreaTagsMarkedLowPriority);

        Task<Dictionary<int, bool>> ClearLearningAreaPriorityFromCache(List<int> userIds);
    }
}