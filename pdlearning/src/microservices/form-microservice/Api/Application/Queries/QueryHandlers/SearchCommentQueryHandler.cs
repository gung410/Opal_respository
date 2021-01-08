using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using CommentEntity = Microservice.Form.Domain.Entities.Comment;

namespace Microservice.Form.Application.Queries
{
    public class SearchCommentQueryHandler : BaseThunderQueryHandler<SearchCommentQuery, PagedResultDto<CommentModel>>
    {
        private readonly IRepository<CommentEntity> _commentRepository;
        private readonly IUserContext _userContext;

        public SearchCommentQueryHandler(IRepository<CommentEntity> commentRepository, IUserContext userContext)
        {
            this._commentRepository = commentRepository;
            _userContext = userContext;
        }

        protected override async Task<PagedResultDto<CommentModel>> HandleAsync(SearchCommentQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _commentRepository.GetAll()
                .Where(c => c.ObjectId == query.Request.ObjectId);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.Request.PagedInfo);

            var entities = await dbQuery
                .Select(p => new CommentModel(p))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CommentModel>(totalCount, entities);
        }
    }
}
