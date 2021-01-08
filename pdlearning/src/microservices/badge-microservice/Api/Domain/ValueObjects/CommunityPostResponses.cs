namespace Microservice.Badge.Domain.ValueObjects
{
    public class CommunityPostResponses
    {
        public string PostId { get; set; }

        public int NumOfResponses { get; set; }

        public int NumOfMultimedia { get; set; }

        public bool HasLink { get; set; }

        public PostStatistic ToCommunityPostStatistic()
        {
            return new PostStatistic
            {
                PostId = PostId,
                HasLink = HasLink,
                NumOfMultimedia = NumOfMultimedia,
                NumOfResponses = NumOfResponses
            };
        }
    }
}
