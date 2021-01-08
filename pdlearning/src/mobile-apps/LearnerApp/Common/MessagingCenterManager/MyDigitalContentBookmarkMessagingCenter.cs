using System;
using Xamarin.Forms;

namespace LearnerApp.Common.MessagingCenterManager
{
    public static class MyDigitalContentBookmarkMessagingCenter
    {
        public const string Key = "my-digital-content-details-bookmarked";

        public static void Subscribe(object sender, Action<object, DigitalContentBookmarkMessagingCenterArgs> action)
        {
            MessagingCenter.Subscribe(sender, Key, action);
        }

        public static void Send(object sender, DigitalContentBookmarkMessagingCenterArgs args)
        {
            MessagingCenter.Send(sender, Key, args);
        }

        public static void Unsubscribe(object sender)
        {
            MessagingCenter.Unsubscribe<object>(sender, Key);
        }
    }

    public class DigitalContentBookmarkMessagingCenterArgs
    {
        public string DigitalContentId { get; set; }

        public bool IsBookmarked { get; set; }
    }
}
