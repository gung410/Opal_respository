using System;
using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetCommentByIdQuery : BaseThunderQuery<CommentModel>
    {
        public Guid Id { get; set; }
    }
}
