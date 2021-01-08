using System;
using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetVideoCommentByIdQuery : BaseThunderQuery<VideoCommentModel>
    {
        public Guid Id { get; set; }
    }
}
