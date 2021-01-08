using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyDigitalContentToMigrateQueryHandler : BaseQueryHandler<GetMyDigitalContentToMigrateQuery, int>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<MyDigitalContent> _myDigitalContentRepository;

        public GetMyDigitalContentToMigrateQueryHandler(
            IThunderCqrs thunderCqrs,
            IRepository<MyDigitalContent> myDigitalContentRepository,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _myDigitalContentRepository = myDigitalContentRepository;
        }

        protected override Task<int> HandleAsync(GetMyDigitalContentToMigrateQuery query, CancellationToken cancellationToken)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new UnauthorizedAccessException("The user must be in role SysAdministrator!");
            }

            if (query.BatchSize == 0)
            {
                throw new BusinessLogicException("Please specify the batch size");
            }

            if (query.Statuses == null || !query.Statuses.Any())
            {
                throw new BusinessLogicException("Please specify the my digital content status.");
            }

            if (query.OriginalObjectIds != null && !query.OriginalObjectIds.Any())
            {
                throw new BusinessLogicException("OriginalObjectIds is empty.");
            }

            if (query.OriginalObjectIds != null && query.OriginalObjectIds.Any())
            {
                return MigrateWithSpecificOriginalObjectIds(query, cancellationToken);
            }

            return MigrateAllMyDigitalContents(query, cancellationToken);
        }

        private async Task<int> MigrateWithSpecificOriginalObjectIds(GetMyDigitalContentToMigrateQuery query, CancellationToken cancellationToken)
        {
            var batchSize = query.BatchSize;
            int processedItems = 0;

            for (int i = 0; i < query.OriginalObjectIds.Count; i += query.BatchSize)
            {
                var originalObjectIds = query.OriginalObjectIds
                    .Skip(i)
                    .Take(batchSize)
                    .ToList();

                var myCourses = await _myDigitalContentRepository
                    .GetAll()
                    .Where(p => query.Statuses.Contains(p.Status))
                    .Where(p => originalObjectIds.Contains(p.DigitalContentId))
                    .ToListAsync(cancellationToken);

                processedItems += myCourses.Count;

                batchSize += query.BatchSize;

                await SendMyDigitalContentChangedEvent(query, myCourses, cancellationToken);
            }

            return processedItems;
        }

        private async Task<int> MigrateAllMyDigitalContents(GetMyDigitalContentToMigrateQuery query, CancellationToken cancellationToken)
        {
            var totalCount = await _myDigitalContentRepository
                .GetAll()
                .CountAsync(cancellationToken);

            var batchSize = query.BatchSize;
            int processedItems = 0;

            for (int i = 0; i < totalCount; i += query.BatchSize)
            {
                var myDigitalContents = await _myDigitalContentRepository
                    .GetAll()
                    .Where(p => query.Statuses.Contains(p.Status))
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(i)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                processedItems += myDigitalContents.Count;

                batchSize += query.BatchSize;

                await SendMyDigitalContentChangedEvent(query, myDigitalContents, cancellationToken);
            }

            return processedItems;
        }

        private async Task SendMyDigitalContentChangedEvent(
            GetMyDigitalContentToMigrateQuery query,
            List<MyDigitalContent> myDigitalContents,
            CancellationToken cancellationToken)
        {
            foreach (var myDigitalContent in myDigitalContents)
            {
                switch (query.MigrationEventType)
                {
                    case MigrationEventType.All:
                        // Support  for search module
                        await _thunderCqrs.SendEvent(
                            new MyDigitalContentChangeEvent(myDigitalContent, myDigitalContent.Status), cancellationToken);

                        // Support for report module
                        await _thunderCqrs.SendEvent(
                            new MyDigitalContentRecordEvent(myDigitalContent), cancellationToken);
                        break;

                    case MigrationEventType.MyDigitalContent:
                        // Support  for search module
                        await _thunderCqrs.SendEvent(
                            new MyDigitalContentChangeEvent(myDigitalContent, myDigitalContent.Status), cancellationToken);
                        break;

                    case MigrationEventType.MyDigitalContentRecord:
                        // Support for report module
                        await _thunderCqrs.SendEvent(
                            new MyDigitalContentRecordEvent(myDigitalContent), cancellationToken);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
