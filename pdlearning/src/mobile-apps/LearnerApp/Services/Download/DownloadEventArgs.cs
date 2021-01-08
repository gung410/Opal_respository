using System;
using LearnerApp.Common;

namespace LearnerApp.Services.Download
{
    public class DownloadEventArgs : EventArgs
    {
        public DownloadState DownloadState;
        public double ProgressPercentage;
        public string ContentId;

        public DownloadEventArgs(DownloadState downloadState, double progressPercentage, string contentId)
        {
            DownloadState = downloadState;
            ProgressPercentage = progressPercentage;
            ContentId = contentId;
        }
    }
}
