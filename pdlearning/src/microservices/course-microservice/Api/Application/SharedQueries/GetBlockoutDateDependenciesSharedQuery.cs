using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.DomainExtensions;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetBlockoutDateDependenciesSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<BlockoutDate> _blockoutDateReadOnlyRepository;

        public GetBlockoutDateDependenciesSharedQuery(IReadOnlyRepository<BlockoutDate> blockoutDateReadOnlyRepository)
        {
            _blockoutDateReadOnlyRepository = blockoutDateReadOnlyRepository;
        }

        public async Task<GetBlockoutDateDependenciesModel> Execute(
            List<string> serviceSchemeIds,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
        {
            var matchedBlockoutDates = await _blockoutDateReadOnlyRepository
                .GetAll()
                .Where(p => p.Status == Domain.Enums.BlockoutDateStatus.Active)
                .Where(BlockoutDateExtensions.IsMatchServiceSchemesQueryExpr(serviceSchemeIds))
                .WhereIf(fromDate != null && toDate == null, BlockoutDate.IsMatchDateExpr(fromDate))
                .WhereIf(fromDate == null && toDate != null, BlockoutDate.IsMatchDateExpr(toDate))
                .WhereIf(
                    fromDate != null && toDate != null,
                    () => BlockoutDate.IsMatchDateRangeExpr(fromDate.GetValueOrDefault(), toDate.GetValueOrDefault()))
                .ToListAsync(cancellationToken);
            return GetBlockoutDateDependenciesModel.Create(matchedBlockoutDates, fromDate, toDate);
        }
    }
}
