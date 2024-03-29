using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Thunder.Platform.Core.Domain.Repositories;
using CommentEntity = Microservice.Form.Domain.Entities.Comment;

namespace Microservice.Form.Application.Queries
{
    public class GetCommentByIdQueryHandler : BaseQueryHandler<GetCommentByIdQuery, CommentModel>
    {
        private readonly IRepository<CommentEntity> _commentRepository;

        public GetCommentByIdQueryHandler(
            IRepository<CommentEntity> commentRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _commentRepository = commentRepository;
        }

        protected override async Task<CommentModel> HandleAsync(GetCommentByIdQuery query, CancellationToken cancellationToken)
        {
            var commentEntity = await this._commentRepository.GetAsync(query.Id);
            var commentModel = new CommentModel(commentEntity);
            return commentModel;
        }
    }
}
