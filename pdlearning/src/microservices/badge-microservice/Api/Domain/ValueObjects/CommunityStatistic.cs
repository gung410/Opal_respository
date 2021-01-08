using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Domain.ValueObjects
{
    /// <summary>
    /// This class defines group statistic information of a user for a community that is executed from a specific date.
    /// </summary>
    public class CommunityStatistic : BaseStatistic
    {
        public CommunityStatistic()
        {
            ExecutedDate = Clock.Now;
        }

        public int NumOfCreatedForum { get; init; }

        public int NumOfInteractions { get; init; }

        public int NumOfSharePost { get; init; }

        public int NumOfCreatedPost { get; init; }
    }
}
