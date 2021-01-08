using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Uploader.Application.RequestDtos;
using Microservice.Uploader.Dtos;

namespace Microservice.Uploader.Services
{
    public interface IAmazonS3UploaderService
    {
        string GetFile(string key);

        List<GetFileResult> GetFiles(string[] keys);

        Task<CreateMultipartUploadSessionResult> CreateMultipartUpload(CreateMultipartUploadSessionRequest request, CancellationToken cancellationToken);

        CreateMultipartPreSignedUrlResult CreateMultipartPreSignedUrl(CreateMultipartPreSignedUrlRequest request);

        Task CompleteMultipartUpload(CompleteMultipartUploadSessionRequest request, CancellationToken cancellationToken);

        Task AbortMultipartUpload(AbortMultipartUploadSessionRequest request, CancellationToken cancellationToken);

        Task<CompleteMultipartFileResult> CompleteMultipartFile(CompleteMultipartFileResquest request, CancellationToken cancellationToken);

        Task ExtractScormPackage(ExtractScormPackageRequest request, CancellationToken cancellationToken);

        Task<ScormProcessingStatusResult> GetScormProcessingStatus(ExtractScormPackageRequest request, CancellationToken cancellationToken);
    }
}
