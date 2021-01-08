using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    /// <summary>
    /// Implements <see cref="IHasStatistic{TEnum}"/>.
    /// </summary>
    public class LearningPathStatisticModel : IHasStatistic<LearningPathType>
    {
        public LearningPathStatisticModel(LearningPathType type, int totalCount)
        {
            Type = type;
            TotalCount = totalCount;
        }

        public LearningPathType Type { get; }

        public int TotalCount { get; }
    }
}
