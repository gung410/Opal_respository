using System;
using System.Linq.Expressions;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class PostStatistic
    {
        public PostStatistic()
        {
            ModifiedDate = DateTime.Now;
        }

        public string PostId { get; set; }

        public Guid? CommunityId { get; set; }

        public Guid CreatedBy { get; set; }

        public int NumOfResponses { get; set; }

        public int NumOfMultimedia { get; set; }

        public bool HasLink { get; set; }

        public DateTime ModifiedDate { get; set; }

        public static Expression<Func<PostStatistic, bool>> IsInCommunity()
        {
            return p => p.CommunityId != null;
        }

        public static Expression<Func<PostStatistic, bool>> IsQualifiedLinkCuratorPost(LinkCuratorBadgeCriteria badgeCriteria)
        {
            return x => x.HasLink && x.NumOfResponses >= badgeCriteria.NumOfResponse;
        }

        public static Expression<Func<PostStatistic, bool>> IsQualifiedConversationStarter(ConversationStarterBadgeCriteria badgeCriteria)
        {
            return x => x.NumOfResponses >= badgeCriteria.NumOfResponse;
        }

        public static Expression<Func<PostStatistic, bool>> IsQualifiedVisualStoryteller(VisualStorytellerBadgeCriteria badgeCriteria)
        {
            return x => x.NumOfMultimedia >= badgeCriteria.NumOfMultimedia && x.NumOfResponses >= badgeCriteria.NumOfResponse;
        }
    }
}
