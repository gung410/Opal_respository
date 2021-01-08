using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Domain.Entities;
using Microservice.Uploader.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Uploader.Application.Queries.QueryHandlers
{
    public class SearchPersonalFilesQueryHandler : BaseQueryHandler<SearchPersonalFilesQuery, PagedResultDto<PersonalFileModel>>
    {
        private readonly IRepository<PersonalFile> _personalFileRepository;

        public SearchPersonalFilesQueryHandler(IAccessControlContext accessControlContext, IRepository<PersonalFile> personalFileRepository, IUnitOfWorkManager unitOfWorkManager)
            : base(accessControlContext, unitOfWorkManager)
        {
            _personalFileRepository = personalFileRepository;
        }

        protected override async Task<PagedResultDto<PersonalFileModel>> HandleAsync(SearchPersonalFilesQuery query, CancellationToken cancellationToken)
        {
            var fileQuery = _personalFileRepository
                .GetAll()
                .Where(f => f.UserId == CurrentUserId)
                .WhereIf(!string.IsNullOrEmpty(query.SearchText), m => m.FileName.Contains(query.SearchText))
                .WhereIf(!query.FilterByType.Contains(FileType.All), m => query.FilterByType.Contains(m.FileType))
                .WhereIf(query.FilterByExtensions != null && query.FilterByExtensions.Any(), m => query.FilterByExtensions.Contains(m.FileExtension));

            var totalCount = await fileQuery.CountAsync(cancellationToken);

            var sortDirection = ConverSortDirection(query.SortDirection);

            fileQuery = ApplySorting(fileQuery, query.PagedInfo, $"{query.SortBy} {sortDirection} ");

            fileQuery = ApplyPaging(fileQuery, query.PagedInfo);

            var entities = await fileQuery.Select(p => new PersonalFileModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<PersonalFileModel>(totalCount, entities);
        }

        private string ConverSortDirection(SortDirection sort)
        {
            return sort switch
            {
                SortDirection.Ascending => "ASC",
                _ => "DESC",
            };
        }
    }
}
