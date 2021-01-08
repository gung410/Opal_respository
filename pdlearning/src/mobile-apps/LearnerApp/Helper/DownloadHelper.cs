using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services.Download;
using LearnerApp.Services.Identity;
using Xamarin.Forms;

namespace LearnerApp.Helper
{
    public class DownloadHelper
    {
        private readonly WebClient _webClient;
        private readonly IPathDownload _pathDownload;
        private EventHandler<DownloadEventArgs> _onFileDownloaded;
        private IIdentityService _identityService;

        public DownloadHelper()
        {
            _webClient = new WebClient();
            _pathDownload = DependencyService.Resolve<IPathDownload>();
            _identityService = DependencyService.Resolve<IIdentityService>();

            _webClient.DownloadProgressChanged += DownloadProgressChanged;
            _webClient.DownloadFileCompleted += Completed;
        }

        ~DownloadHelper()
        {
            CleanUp();
        }

        public async Task StartDownloadFile(DownloadInfoModel downloadInfo, EventHandler<DownloadEventArgs> onFileDownloaded, string folderName = null)
        {
            folderName ??= _pathDownload.GetPath("Download");
            _onFileDownloaded = onFileDownloaded;

            var accountProperties = await _identityService.GetAccountPropertiesAsync();
            if (accountProperties != null)
            {
                _webClient.Headers.Add("Authorization", "Bearer " + accountProperties.AccessToken);
            }

            Directory.CreateDirectory(folderName);
            try
            {
                string dateTime = DateTime.Now.ToString("ddMMyyyyHHmmss");
                string extension = Path.GetExtension(downloadInfo.FileName);
                int position = downloadInfo.FileName.IndexOf(extension, StringComparison.Ordinal);
                string name = downloadInfo.FileName.Substring(0, position);
                string pathToNewFile = $"{folderName}/{name}_{dateTime}{extension}";
                _webClient.DownloadFileAsync(new Uri(downloadInfo.Url), pathToNewFile);
            }
            catch (Exception)
            {
                _onFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.Failed, 0, null));
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _onFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.None, e.ProgressPercentage, null));
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _onFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.Failed, 100, null));
            }
            else
            {
                _onFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.Completed, 100, null));
            }

            CleanUp();
        }

        private void CleanUp()
        {
            _onFileDownloaded = null;
            _webClient.DownloadProgressChanged -= DownloadProgressChanged;
            _webClient.DownloadFileCompleted -= Completed;
        }
    }
}
