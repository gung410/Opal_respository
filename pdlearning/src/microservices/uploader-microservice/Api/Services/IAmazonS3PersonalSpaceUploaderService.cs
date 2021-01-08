using System.Threading;
using System.Threading.Tasks;
using Microservice.Uploader.Application.RequestDtos;
using Microservice.Uploader.Dtos;

namespace Microservice.Uploader.Services
{
    public interface IAmazonS3PersonalSpaceUploaderService
    {
        Task<CreateMultipartUploadSessionResult> CreateMultipartUpload(CreateMultipartUploadSessionRequest request, CancellationToken cancellationToken);

        CreateMultipartPreSignedUrlResult CreateMultipartPreSignedUrl(CreateMultipartPreSignedUrlRequest request);

        Task CompleteMultipartUpload(CompleteMultipartUploadSessionRequest request, CancellationToken cancellationToken);

        Task AbortMultipartUpload(AbortMultipartUploadSessionRequest request, CancellationToken cancellationToken);

        Task<CompleteMultipartFileResult> CompleteMultipartFile(CompleteMultipartFileResquest request, CancellationToken cancellationToken);
    }
}
