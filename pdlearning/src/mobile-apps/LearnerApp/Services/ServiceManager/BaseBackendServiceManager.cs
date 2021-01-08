using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace LearnerApp.Services.ServiceManager
{
    public abstract class BaseBackendServiceManager
    {
        protected async Task DownloadFile(
            string filePath,
            Func<Task<HttpContent>> getFileTask)
        {
            var httpContent = await getFileTask();

            int bufferSize = 1024 * 32;
            using (Stream contentStream = await httpContent.ReadAsStreamAsync(),
                fileStream = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Inheritable,
                    bufferSize,
                    true))
            {
                byte[] buffer = new byte[bufferSize];
                int read;
                do
                {
                    read = await contentStream.ReadAsync(buffer, 0, bufferSize);
                    await fileStream.WriteAsync(buffer, 0, read);
                }
                while (read > 0);
                fileStream.Close();
                contentStream.Close();
            }
        }
    }
}
