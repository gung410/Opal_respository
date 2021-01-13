using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Core.Domain.Repositories;
using CommentEntity = Microservice.StandaloneSurvey.Domain.Entities.Comment;

namespace Microservice.StandaloneSurvey.Application.Queries
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