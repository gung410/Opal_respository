using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Search.Messages;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Search
{
    [OpalConsumer("usersearch.migrate")]
    public class Search_UserSearchMigrateConsumer : ScopedOpalMessageConsumer<List<Search_UserSearchMigrateMessageItem>>
    {
        private readonly ILogger<Search_UserSearchMigrateConsumer> _logger;
        private readonly IRepository<SearchEngine> _searchEngineRepository;
        private readonly IRepository<SAM_UserHistory> _userHistoryRepository;

        public Search_UserSearchMigrateConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SearchEngine> searchEngineRepository,
            IRepository<SAM_UserHistory> userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<Search_UserSearchMigrateConsumer>();
            _searchEngineRepository = searchEngineRepository;
            _userHistoryRepository = userHistoryRepository;
        }

        public async Task InternalHandleAsync(List<Search_UserSearchMigrateMessageItem> searches)
        {
            if (searches == null || !searches.Any())
            {
                return;
            }

            var searchEngineRecords = new List<SearchEngine>();

            foreach (var searchItem in searches)
            {
                var latestHistoryItem = (await _userHistoryRepository.GetAllListAsync(t => t.UserId == searchItem.UserId))
                    .OrderByDescending(t => t.No)
                    .FirstOrDefault();

                if (latestHistoryItem == null)
                {
                    _logger.LogWarning($"Latest user history with UserId {searchItem.UserId} does not exist");
                    continue;
                }

                var isExistItem = (await _searchEngineRepository.FirstOrDefaultAsync(t => t.Id == searchItem.Id)) != null;

                if (!isExistItem)
                {
                    searchEngineRecords.Add(new SearchEngine()
                    {
                        Id = searchItem.Id,
                        UserId = searchItem.UserId,
                        UserHistoryId = latestHistoryItem.Id,
                        DepartmentId = latestHistoryItem.DepartmentId,
                        SearchText = searchItem.Keyword,
                        CreatedDate = searchItem.CreatedUtc
                    });
                }
            }

            await _searchEngineRepository.InsertManyAsync(searchEngineRecords);
        }
    }
}
