using System;
using System.Threading;
using System.Threading.Tasks;
using LearnerApp.Controls;
using Rg.Plugins.Popup.Services;

namespace LearnerApp.Services.Dialog
{
    public class LoadingPopupController : IDisposable
    {
        private readonly LoadingIndicator _loadingIndicator = new LoadingIndicator();
        private bool _isLoading = false;
        private int _popInMilliseconds;

        public Task Loading(int popInMilliseconds)
        {
            _isLoading = true;
            _popInMilliseconds = popInMilliseconds;

            return PopupNavigation.Instance.PushAsync(_loadingIndicator);
        }

        public void Dispose()
        {
            if (_isLoading)
            {
                if (_popInMilliseconds > 0)
                {
                    Thread.Sleep(_popInMilliseconds);
                }

                try
                {
                    PopupNavigation.Instance.RemovePageAsync(_loadingIndicator);
                }
                catch (InvalidOperationException)
                {
                    // Somehow if the page is already remove
                }
            }
        }
    }
}
