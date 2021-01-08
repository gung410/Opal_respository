using System.Threading;
using System.Threading.Tasks;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Application.RequestDtos;

namespace Microservice.Uploader.Services
{
    public interface IDownloadLearningContentService
    {
        Task<DownloadLearningContentResultModel> DownloadLearningContent(DownloadLearningContentRequest request, CancellationToken cancellationToken);
    }
}
