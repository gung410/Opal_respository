using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;

namespace LearnerApp.Services
{
    public interface IS3UploadService
    {
        /// <summary>
        /// Upload file to S3 AWS.
        /// </summary>
        /// <param name="file">File data.</param>
        /// <param name="folder">The folder data save to.</param>
        /// <returns>File path location.</returns>
        Task<string> UploadFile(FileData file, string folder);

        Task<string> UploadFile(MediaFile file, string folder);
    }
}
