using System;
using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetCommentByIdQuery : BaseThunderQuery<CommentModel>
    {
        public Guid Id { get; set; }
    }
}