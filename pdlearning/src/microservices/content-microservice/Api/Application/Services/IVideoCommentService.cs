using System;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.Services
{
    public interface IVideoCommentService
    {
        Task<VideoCommentModel> CreateVideoComment(CreateVideoCommentRequest request, Guid userId);

        Task<VideoCommentModel> UpdateVideoComment(UpdateVideoCommentRequest request, Guid userId);

        Task DeleteVideoComment(Guid id, Guid userId);

        Task<PagedResultDto<VideoCommentModel>> SearchVideoComments(SearchVideoCommentRequest dto, Guid userId);
    }
}
