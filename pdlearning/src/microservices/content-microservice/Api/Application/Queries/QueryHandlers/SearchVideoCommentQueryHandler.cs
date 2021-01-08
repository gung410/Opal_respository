using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using DigitalContentEntity = Microservice.Content.Domain.Entities.DigitalContent;
using VideoCommentEntity = Microservice.Content.Domain.Entities.VideoComment;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class SearchVideoCommentQueryHandler : BaseThunderQueryHandler<SearchVideoCommentQuery, PagedResultDto<VideoCommentModel>>
    {
        private readonly IRepository<DigitalContentEntity> _digitalContentRepository;
        private readonly IRepository<VideoCommentEntity> _videoCommentRepository;

        public SearchVideoCommentQueryHandler(IRepository<VideoCommentEntity> videoCommentRepository, IRepository<DigitalContentEntity> digitalContentRepository)
        {
            _videoCommentRepository = videoCommentRepository;
            _digitalContentRepository = digitalContentRepository;
        }

        protected override async Task<PagedResultDto<VideoCommentModel>> HandleAsync(SearchVideoCommentQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _videoCommentRepository
                .GetAll()
                .Where(c => c.SourceType == query.Request.SourceType);

            switch (query.Request.SourceType)
            {
                case VideoSourceType.CCPM:
                    if (Guid.Equals(query.Request.ObjectId, Guid.Empty))
                    {
                        return new PagedResultDto<VideoCommentModel>(0, new List<VideoCommentModel>());
                    }

                    var digitalContent = await _digitalContentRepository.GetAsync(query.Request.ObjectId.Value);
                    dbQuery = dbQuery.Where(c => c.OriginalObjectId == digitalContent.OriginalObjectId && c.VideoId == query.Request.VideoId);
                    break;
                case VideoSourceType.LMM:
                    dbQuery = dbQuery.Where(c => c.OriginalObjectId == query.Request.OriginalObjectId.Value
                                              && c.ObjectId == query.Request.ObjectId.Value);

                    dbQuery = dbQuery.Where(c => c.VideoId == query.Request.VideoId);
                    break;
                case VideoSourceType.CSL:
                    dbQuery = dbQuery.Where(c => c.VideoId == query.Request.VideoId);
                    break;
                default:
                    break;
            }

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            switch (query.Request.OrderBy)
            {
                case Domain.ValueObject.VideoCommentOrderBy.CreatedDate:
                    dbQuery = query.Request.OrderType == Domain.ValueObject.OrderType.ASC ? dbQuery.OrderBy(p => p.CreatedDate) : dbQuery.OrderByDescending(p => p.CreatedDate);
                    break;
                case Domain.ValueObject.VideoCommentOrderBy.VideoTime:
                    dbQuery = query.Request.OrderType == Domain.ValueObject.OrderType.DESC ? dbQuery.OrderByDescending(p => p.VideoTime) : dbQuery.OrderBy(p => p.VideoTime);
                    break;
                default:
                    dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);
                    break;
            }

            var pageInfo = new PagedResultRequestDto
            {
                MaxResultCount = query.Request.MaxResultCount,
                SkipCount = query.Request.SkipCount
            };

            dbQuery = ApplyPaging(dbQuery, pageInfo);

            var entities = await dbQuery
                .Select(p => new VideoCommentModel(p))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<VideoCommentModel>(totalCount, entities);
        }
    }
}
