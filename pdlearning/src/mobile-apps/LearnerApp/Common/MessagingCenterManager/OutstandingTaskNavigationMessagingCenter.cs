using System;
using Xamarin.Forms;

namespace LearnerApp.Common.MessagingCenterManager
{
    public static class OutstandingTaskNavigationMessagingCenter
    {
        private static string NavigatedToKey => $"{nameof(OutstandingTaskNavigationMessagingCenter)}-{nameof(OutstandingTaskNavigatedToArguments)}";

        public static void SubscribeOnNavigatedTo(object sender, Action<object, OutstandingTaskNavigatedToArguments> action)
        {
            MessagingCenter.Subscribe(sender, NavigatedToKey, action);
        }

        public static void SendOnNavigatedTo(object sender, OutstandingTaskNavigatedToArguments args)
        {
            MessagingCenter.Send(sender, NavigatedToKey, args);
        }

        public static void UnsubscribeOnNavigatedTo(object sender)
        {
            MessagingCenter.Unsubscribe<object>(sender, NavigatedToKey);
        }
    }

    public class OutstandingTaskNavigatedToArguments
    {
        public OutstandingTaskNavigatedToArguments(string outstandingTaskId)
        {
            OutstandingTaskId = outstandingTaskId;
        }

        public string OutstandingTaskId { get; set; }
    }
}
