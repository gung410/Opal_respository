using System;
using System.Collections.Generic;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;

namespace LearnerApp.Services
{
    public static class DigitalContentCardBuilder
    {
        public static List<ItemCard> BuildDigitalContentCardList(List<MyDigitalContentSummary> digitalContentSummary, List<MyDigitalContentDetails> digitalContentDetails)
        {
            return digitalContentSummary.Join(
                digitalContentDetails,
                summary => summary.DigitalContentId,
                details => details.Id,
                (summary, details) =>
                new ItemCard
                {
                    Id = summary.DigitalContentId,
                    Name = details.Title,
                    ReviewsCount = summary.ReviewsCount,
                    Rating = summary.Rating,
                    Status = summary.MyDigitalContent?.Status,
                    CourseStatus = details.Status,
                    CardType = BookmarkType.DigitalContent,
                    OriginalObjectId = details.OriginalObjectId,
                    BookmarkInfo = summary.BookmarkInfo,
                    Tags = new List<string> { SeparateStringByUppercase.Convert(BookmarkType.DigitalContent.ToString()) },
                    ThumbnailUrl = GetDocumentTypeImage(details.FileExtension),
                    IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark(),
                    IsShowClassRunConfirm = false,
                    IsShowClassRunReject = false,
                    BookmarkInfoChangedDate = summary.BookmarkInfo != null ? summary.BookmarkInfo.CreatedDate : DateTime.MinValue
                }).ToList();
        }

        public static string GetDocumentTypeImage(string extension)
        {
            return string.IsNullOrEmpty(extension) ? "document.svg" :
                extension == "picture-file" ? "picture_file.svg" : $"{extension}.svg";
        }
    }
}
