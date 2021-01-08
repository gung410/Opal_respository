using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Adapter.Assessment
{
    public interface IAssessmentAdapter
    {
        List<dynamic> GetAssessmentConfigurations(int ownerId,
            List<int> activityIds = null,
            List<string> activityExtIds = null,
            List<int> surveyIds = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            bool includeStatusType = false);

        List<dynamic> GetAssessmentProfiles(int ownerId, int customerId, List<int> userIds,
            List<int> activityIds = null,
            List<string> actvityExtIds = null,
            List<int> statustypeIds = null,
            List<int> entityStatusIds = null,
            DateTime? surveyStartDateBefore = null,
            DateTime? surveyStartDateAfter = null,
            DateTime? surveyEndDateBefore = null,
            DateTime? surveyEndDateAfter = null,
            List<string> alternativeExtIdsToIncludeAnswer = null,
            string displayLocale = "",
            string fallbackDisplayLocale = "",
            string answerLocale = "",
            bool defaultCurrentSurveyIfNoFiltering = true,
            bool getLatestResultOnActivityOfUserOnly = false);

        List<dynamic> GetLevelGroups(int ownerId, int customerId, List<int> activityIds = null,
            List<int> levelgroupIds = null,
            List<int> departmentIds = null,
            List<int> roleIds = null,
            List<string> tags = null,
            bool includeLocalizedData = false);

        List<dynamic> GetLevelLimits(int ownerId, int customerId, List<int> activityIds = null,
            List<int> levelLimitIds = null,
            List<int> levelGroupIds = null,
            List<int> categoryIds = null,
            List<int> questionIds = null,
            List<int> itemIds = null,
            List<string> levelGroupTags = null,
            bool includeLocalizedData = false);

        /// <summary>
        /// Gets the learning area priorities from the latest published Learning Needs Analysis.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="customerId"></param>
        /// <param name="userId"></param>
        /// <param name="activityExtId"></param>
        /// <param name="statusTypeCodeNames"></param>
        /// <param name="learningAreaPriorityPropertyPrefix"></param>
        /// <returns></returns>
        Task<LearningAreaPriority> GetLearningAreaPrioritiesFromLatestPublishedAnswer(
            int ownerId, int customerId,
            int userId,
            string activityExtId,
            List<string> statusTypeCodeNames,
            string learningAreaPriorityPropertyPrefix
            );
    }
}
