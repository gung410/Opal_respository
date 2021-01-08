using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using CommentEntity = Microservice.Content.Domain.Entities.Comment;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetCommentByIdQueryHandler : BaseThunderQueryHandler<GetCommentByIdQuery, CommentModel>
    {
        private readonly IRepository<CommentEntity> _commentRepository;

        public GetCommentByIdQueryHandler(IRepository<CommentEntity> commentRepository)
        {
            _commentRepository = commentRepository;
        }

        protected override async Task<CommentModel> HandleAsync(GetCommentByIdQuery query, CancellationToken cancellationToken)
        {
            var commentEntity = await this._commentRepository.GetAsync(query.Id);
            CommentModel commentModel = new CommentModel(commentEntity);
            return commentModel;
        }
    }
}
