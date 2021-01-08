using System;
using LearnerApp.Common;
using LearnerApp.Resources.Texts;

namespace LearnerApp.Models.MyLearning
{
    public class MyDigitalContentSummary
    {
        public string DigitalContentId { get; set; }

        public double Rating { get; set; }

        public int ReviewsCount { get; set; }

        public BookmarkInfo BookmarkInfo { get; set; }

        public MyDigitalContent MyDigitalContent { get; set; }

        public int ViewsCount { get; set; }

        public int DownloadsCount { get; set; }

        public int SharesCount { get; set; }

        public int TotalLike { get; set; }

        public bool IsLike { get; set; }

        public string GetDigitalContentStatus()
        {
            if (MyDigitalContent == null)
            {
                return string.Empty;
            }

            Enum.TryParse(MyDigitalContent.Status, out StatusLearning value);
            return value.ToString();
        }

        public string GetDigitalContentText()
        {
            if (MyDigitalContent == null)
            {
                return TextsResource.START_LEARNING;
            }

            Enum.TryParse(MyDigitalContent.Status, out StatusLearning value);
            var status = value switch
            {
                StatusLearning.InProgress => TextsResource.CONTINUE,
                StatusLearning.Completed => TextsResource.LEARN_AGAIN,
                _ => TextsResource.START_LEARNING,
            };
            return status;
        }
    }
}
