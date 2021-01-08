using System.Collections.Generic;

namespace LearnerApp.Models.Learner
{
    public class MyLearningSearchResultDto<T> : ListResultDto<T>
    {
        public List<SearchStatistics> Statistics { get; set; }
    }
}
