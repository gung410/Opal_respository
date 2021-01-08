using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using ChapterEntity = Microservice.Content.Domain.Entities.Chapter;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class SearchVideoChapterQueryHandler : BaseThunderQueryHandler<SearchVideoChapterQuery, List<ChapterModel>>
    {
        private readonly IRepository<ChapterEntity> _chapterRepository;

        public SearchVideoChapterQueryHandler(IRepository<ChapterEntity> chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        protected override async Task<List<ChapterModel>> HandleAsync(SearchVideoChapterQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _chapterRepository
                .GetAllIncluding(chapter => chapter.Attachments)
                .Where(c => c.SourceType == query.Request.SourceType && c.ObjectId == query.Request.ObjectId);

            return await dbQuery
                .Select(c => new ChapterModel(c))
                .ToListAsync(cancellationToken);
        }
    }
}
