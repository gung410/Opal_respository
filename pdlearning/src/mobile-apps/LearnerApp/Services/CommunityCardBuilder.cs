using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Learner;

namespace LearnerApp.Services
{
    public static class CommunityCardBuilder
    {
        public static List<ItemCard> BuildCommunityCardListAsync(List<Community> communities, List<BookmarkInfo> bookmarkedList)
        {
            List<ItemCard> communityCards = new List<ItemCard>();
            foreach (var item in communities)
            {
                communityCards.Add(new ItemCard
                {
                    Id = item.Id,
                    Guid = item.Guid,
                    Name = item.Name,
                    Description = item.Description,
                    BookmarkInfo = bookmarkedList.IsNullOrEmpty() ? null : bookmarkedList.FirstOrDefault(p => p.ItemId == item.Guid),
                    MemberCount = item.Members,
                    DetailUrl = item.Url,
                    IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark(),
                    CardType = BookmarkType.Community
                });
            }

            return communityCards;
        }
    }
}
