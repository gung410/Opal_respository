using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using CommentEntity = Microservice.Content.Domain.Entities.Comment;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class SearchCommentQueryHandler : BaseThunderQueryHandler<SearchCommentQuery, PagedResultDto<CommentModel>>
    {
        private readonly IRepository<CommentEntity> _commentRepository;

        public SearchCommentQueryHandler(IRepository<CommentEntity> commentRepository)
        {
            this._commentRepository = commentRepository;
        }

        protected override async Task<PagedResultDto<CommentModel>> HandleAsync(SearchCommentQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _commentRepository
                .GetAll()
                .Where(c => c.ObjectId == query.Request.ObjectId);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.Request.PagedInfo);

            var entities = await dbQuery.Select(p => new CommentModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<CommentModel>(totalCount, entities);
        }
    }
}
