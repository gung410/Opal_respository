using System;
using Microservice.Content.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.RequestDtos
{
    public class SearchVideoCommentRequest : PagedResultRequestDto
    {
        public Guid? ObjectId { get; set; } = Guid.Empty;

        public Guid? OriginalObjectId { get; set; } = Guid.Empty;

        public VideoSourceType SourceType { get; set; }

        public Guid VideoId { get; set; }

        public VideoCommentOrderBy? OrderBy { get; set; }

        public OrderType? OrderType { get; set; }
    }
}
