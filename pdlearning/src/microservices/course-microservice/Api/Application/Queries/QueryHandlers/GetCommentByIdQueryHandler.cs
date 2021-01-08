using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetCommentByIdQueryHandler : BaseQueryHandler<GetCommentByIdQuery, CommentModel>
    {
        private readonly IReadOnlyRepository<Comment> _readCommentRepository;

        public GetCommentByIdQueryHandler(
            IReadOnlyRepository<Comment> readCommentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCommentRepository = readCommentRepository;
        }

        protected override async Task<CommentModel> HandleAsync(GetCommentByIdQuery query, CancellationToken cancellationToken)
        {
            var commentEntity = await _readCommentRepository.GetAsync(query.Id);
            CommentModel commentModel = new CommentModel(commentEntity);
            return commentModel;
        }
    }
}
