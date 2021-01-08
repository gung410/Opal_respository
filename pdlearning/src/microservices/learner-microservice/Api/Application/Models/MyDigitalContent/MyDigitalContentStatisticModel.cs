using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    /// <summary>
    /// Implements <see cref="IHasStatistic{TEnum}"/>.
    /// </summary>
    public class MyDigitalContentStatisticModel : IHasStatistic<MyDigitalContentStatus>
    {
        public MyDigitalContentStatisticModel(MyDigitalContentStatus type, int totalCount)
        {
            Type = type;
            TotalCount = totalCount;
        }

        public MyDigitalContentStatus Type { get; }

        public int TotalCount { get; }
    }
}
