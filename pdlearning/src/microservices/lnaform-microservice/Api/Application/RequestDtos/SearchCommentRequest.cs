using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class SearchCommentRequest
    {
        public Guid ObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
