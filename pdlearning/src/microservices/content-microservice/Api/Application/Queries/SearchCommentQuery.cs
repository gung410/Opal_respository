using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class SearchCommentQuery : BaseThunderQuery<PagedResultDto<CommentModel>>
    {
        public SearchCommentRequest Request { get; set; }
    }
}
