using System;

namespace Microservice.Learner.Application.Models
{
    public class UserBookmarkedModel
    {
        public UserBookmarkedModel()
        {
        }

        public UserBookmarkedModel(Guid itemId, int totalCount)
        {
            CountTotal = totalCount;
            ItemId = itemId;
        }

        public Guid ItemId { get; set; }

        public int CountTotal { get; set; }
    }
}
