using System;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.Services
{
    public interface ICommentService
    {
        Task<CommentModel> CreateComment(CreateCommentRequest request, Guid userId);

        Task<PagedResultDto<CommentModel>> SearchComments(SearchCommentRequest dto);
    }
}
