using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using VideoCommentEntity = Microservice.Content.Domain.Entities.VideoComment;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetVideoCommentByIdQueryHandler : BaseThunderQueryHandler<GetVideoCommentByIdQuery, VideoCommentModel>
    {
        private readonly IRepository<VideoCommentEntity> _videocommentRepository;

        public GetVideoCommentByIdQueryHandler(IRepository<VideoCommentEntity> commentRepository)
        {
            _videocommentRepository = commentRepository;
        }

        protected override async Task<VideoCommentModel> HandleAsync(GetVideoCommentByIdQuery query, CancellationToken cancellationToken)
        {
            var commentEntity = await this._videocommentRepository.GetAsync(query.Id);
            var commentModel = new VideoCommentModel(commentEntity);
            return commentModel;
        }
    }
}
