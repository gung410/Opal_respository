using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using LearnerApp.Common;

namespace LearnerApp.Services.Download
{
    public class Downloader : IDownloader
    {
        private const int PercentComplete = 100;

        private string _contentId;

        public event EventHandler<DownloadEventArgs> OnFileDownloaded;

        public void DownloadFile(string downloadUrl, string fileName, string contentId, string folder)
        {
            _contentId = contentId;
            Directory.CreateDirectory(folder);

            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadProgressChanged += DownloadProgressChanged;
                webClient.DownloadFileCompleted += Completed;
                string dateTime = DateTime.Now.ToString("ddMMyyyyHHmmss");
                string extension = Path.GetExtension(fileName);
                int position = fileName.IndexOf(extension, StringComparison.Ordinal);
                string name = fileName.Substring(0, position);
                string pathToNewFile = $"{folder}/{name}_{dateTime}{extension}";
                webClient.DownloadFileAsync(new Uri(downloadUrl), pathToNewFile);
            }
            catch (Exception)
            {
                OnFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.Failed, PercentComplete, _contentId));
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.None, e.ProgressPercentage, _contentId));
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OnFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.Failed, PercentComplete, _contentId));
            }
            else
            {
                OnFileDownloaded?.Invoke(this, new DownloadEventArgs(DownloadState.Completed, PercentComplete, _contentId));
            }
        }
    }
}
