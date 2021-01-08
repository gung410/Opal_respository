using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class SearchCommentQuery : BaseThunderQuery<PagedResultDto<CommentModel>>
    {
        public SearchCommentRequest Request { get; set; }
    }
}
