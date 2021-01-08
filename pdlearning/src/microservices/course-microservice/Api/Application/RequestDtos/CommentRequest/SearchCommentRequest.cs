using System;
using Microservice.Course.Application.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchCommentRequest
    {
        public Guid ObjectId { get; set; }

        public EntityCommentType EntityCommentType { get; set; }

        public string ActionType { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
