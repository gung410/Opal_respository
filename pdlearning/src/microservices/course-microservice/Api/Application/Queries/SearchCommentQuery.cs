using System;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchCommentQuery : BaseThunderQuery<PagedResultDto<CommentModel>>
    {
        public Guid ObjectId { get; set; }

        public EntityCommentType EntityCommentType { get; set; }

        public string ActionType { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
