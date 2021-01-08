using System;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.Models
{
    public class DigitalContentModel
    {
        public DigitalContentModel()
        {
        }

        public DigitalContentModel(
            Guid digitalContentId,
            double rating,
            int reviewsCount)
        {
            DigitalContentId = digitalContentId;
            Rating = rating;
            ReviewsCount = reviewsCount;
        }

        public Guid DigitalContentId { get; }

        public double Rating { get; }

        public int ReviewsCount { get; }

        public int ViewsCount { get; private set; }

        public int DownloadsCount { get; private set; }

        public UserBookmarkModel BookmarkInfo { get; private set; }

        public MyDigitalContentModel MyDigitalContent { get; set; }

        public static DigitalContentModel New(
            Guid digitalContentId,
            double rating = 0,
            int reviewsCount = 0)
        {
            return new DigitalContentModel(
                digitalContentId,
                rating,
                reviewsCount);
        }

        public DigitalContentModel WithBookmarkInfo(UserBookmark userBookmark)
        {
            if (userBookmark != null)
            {
                BookmarkInfo = new UserBookmarkModel(userBookmark);
            }

            return this;
        }

        public DigitalContentModel WithMyDigitalContent(MyDigitalContent myDigitalContent)
        {
            if (myDigitalContent != null)
            {
                MyDigitalContent = new MyDigitalContentModel(myDigitalContent);
            }

            return this;
        }

        public DigitalContentModel WithViewsCount(int viewsCount)
        {
            ViewsCount = viewsCount;
            return this;
        }

        public DigitalContentModel WithDownloadsCount(int downloadsCount)
        {
            DownloadsCount = downloadsCount;
            return this;
        }
    }
}
