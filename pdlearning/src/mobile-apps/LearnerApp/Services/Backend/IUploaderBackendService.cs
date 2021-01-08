using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Upload;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IUploaderBackendService
    {
        [Post("/api/uploader/getFiles")]
        Task<List<Thumbnail>> GetThumbnails([Body] string[] identifiers);

        [Get("/api/uploader/getFile?key={thumbnailKey}")]
        Task<string> GetFile(string thumbnailKey);

        [Post("/api/uploader/createMultipartUploadSession")]
        Task<UploadSession> CreateMultipartUploadSession([Body] object param);

        [Post("/api/uploader/createMultipartPreSignedUrl")]
        Task<UploadParts> CreateMultipartPreSignedUrl([Body] object param);

        [Post("/api/uploader/completeMultipartUploadSession")]
        Task CompleteMultipartUploadSession([Body] object param);

        [Post("/api/uploader/completeMultipartFile")]
        Task<CompleteFileUpload> CompleteMultipartFile([Body] object param);
    }
}
