using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    /// <summary>
    /// Implements <see cref="IHasStatistic{TEnum}"/>.
    /// </summary>
    public class MyCourseStatisticModel : IHasStatistic<MyLearningStatus>
    {
        public MyCourseStatisticModel(MyLearningStatus type, int totalCount)
        {
            Type = type;
            TotalCount = totalCount;
        }

        public MyLearningStatus Type { get; set; }

        public int TotalCount { get; set; }
    }
}
