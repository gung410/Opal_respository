namespace LearnerApp.Models
{
    public class DownloadInfoModel
    {
        public DownloadInfoModel(string url, string fileName)
        {
            Url = url;
            FileName = fileName;
        }

        public string Url { get; set; }

        public string FileName { get; set; }
    }
}
