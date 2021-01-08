using System.Collections.Generic;

namespace LearnerApp.Models
{
    public class SearchGroupFilterData
    {
        /// <summary>
        /// List filter value in dictionary for filter.
        /// </summary>
        public Dictionary<string, int> Data { get; set; }

        /// <summary>
        /// Current status for filter forcus.
        /// </summary>
        public string CurrentFilter { get; set; }

        /// <summary>
        /// Display keyword in search result.
        /// </summary>
        public string Keyword { get; set; }
    }
}
