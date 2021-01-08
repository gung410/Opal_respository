using System;
using System.Collections.Generic;

namespace cxOrganization.Business.PDPlanner
{
    public class LearningNeedsAnalysisConfig
    {
        /// <summary>
        /// The external identifier of the "Learning Needs Analysis" activity.
        /// </summary>
        public string ActivityExtId { get; set; }

        /// <summary>
        /// The list of code names of the status types which usable for reporting. e.g: Recommendation.
        /// </summary>
        public List<string> StatusTypeCodeNamesUsableForReport { get; set; }

        /// <summary>
        /// The learning area priority property prefix in the Json Answer.
        /// </summary>
        public string JsonAnswerLearningAreaPriorityPropertyPrefix { get; set; }

        /// <summary>
        /// Get or set value to enable cache for learning area priority
        /// </summary>
        public bool CacheLearningAreaPriority { get; set; }

        /// <summary>
        /// Duration of cache item 
        /// </summary>
        public TimeSpan? CacheDuration { get; set; }
    }
}
