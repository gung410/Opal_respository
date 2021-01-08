using System;
using LearnerApp.Models;
using Xamarin.Forms;

namespace LearnerApp.Common.MessagingCenterManager
{
    public static class CourseBookmarkMessagingCenter
    {
        public const string Key = "course-details-bookmarked";

        public static void Subscribe(object sender, Action<object, CourseBookmarkMessagingCenterArgs> action)
        {
            MessagingCenter.Subscribe(sender, Key, action);
        }

        public static void Send(object sender, CourseBookmarkMessagingCenterArgs args)
        {
            MessagingCenter.Send(sender, Key, args);
        }

        public static void Unsubscribe(object sender)
        {
            MessagingCenter.Unsubscribe<object>(sender, Key);
        }
    }

    public class CourseBookmarkMessagingCenterArgs
    {
        public string CourseId { get; set; }

        public bool IsBookmarked { get; set; }
    }
}
