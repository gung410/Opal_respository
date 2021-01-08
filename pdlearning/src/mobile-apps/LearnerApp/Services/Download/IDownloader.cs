using System;

namespace LearnerApp.Services.Download
{
    public interface IDownloader
    {
        event EventHandler<DownloadEventArgs> OnFileDownloaded;

        void DownloadFile(string downloadUrl, string fileName, string contentId, string folder);
    }
}
