using cxPlatform.Core;
using System.IO;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services.StorageServices
{
    public interface IFileStorageService
    {
        Task<Stream> DownloadFileAsync(string filePath);
        Task<string> UploadFileAsync(IWorkContext workContext, byte[] data, string filePath);
        char FilePathDelimiter { get; }
    }
}
