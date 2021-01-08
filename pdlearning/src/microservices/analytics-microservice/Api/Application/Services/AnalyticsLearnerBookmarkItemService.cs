using System;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.Entities;
using Microservice.Analytics.Domain.ValueObject;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Services
{
#pragma warning disable SA1402 // File may only contain a single type

    public class AnalyticLearnerBookmarkDigitalContentService : AnalyticsShareService, IAnalyticsLearnerService
    {
        private readonly ILogger<AnalyticLearnerBookmarkDigitalContentService> _logger;
        private readonly IRepository<Learner_UserBookmarksDigitalContent> _userBookmarksDigitalContentRepository;

        public AnalyticLearnerBookmarkDigitalContentService(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserBookmarksDigitalContent> userBookmarksDigitalContentRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticLearnerBookmarkDigitalContentService>();
            _userBookmarksDigitalContentRepository = userBookmarksDigitalContentRepository;
        }

        public async Task CreateOrSetToDateBookmarkItem(
            Guid itemId,
            Guid userId,
            bool isUnBookmark,
            AnalyticLearnerBookmarkItemType itemType,
            DateTime time,
            SAM_UserHistory userHistory)
        {
            if (isUnBookmark)
            {
                var bookmarkItems = await _userBookmarksDigitalContentRepository
                    .GetAllListAsync(
                        x => x.DigitalContentId == itemId &&
                             x.UserId == userId &&
                             x.ToDate == null);

                if (bookmarkItems.Any())
                {
                    foreach (var bookmarkItem in bookmarkItems)
                    {
                        bookmarkItem.ToDate = time;
                        bookmarkItem.ChangedBy = userId;
                    }

                    await _userBookmarksDigitalContentRepository.UpdateManyAsync(bookmarkItems);
                }
                else
                {
                    _logger.LogWarning($"Bookmark digital content {itemId} of user {userId} does not exist");
                }
            }
            else
            {
                if (userHistory == null)
                {
                    _logger.LogError("UserHistory cannot be null");
                    return;
                }

                await _userBookmarksDigitalContentRepository.InsertAsync(new Learner_UserBookmarksDigitalContent
                {
                    UserId = userId,
                    UserHistoryId = userHistory.Id.ToString(),
                    DepartmentId = userHistory.DepartmentId,
                    DigitalContentId = itemId,
                    CreatedBy = userId,
                    FromDate = time
                });
            }
        }
    }

    public class AnalyticLearnerBookmarkCourseService : AnalyticsShareService, IAnalyticsLearnerService
    {
        private readonly ILogger<AnalyticLearnerBookmarkCourseService> _logger;
        private readonly IRepository<Learner_UserBookmarksCourse> _userBookmarksDigitalContentRepository;

        public AnalyticLearnerBookmarkCourseService(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserBookmarksCourse> userBookmarksCourseRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticLearnerBookmarkCourseService>();
            _userBookmarksDigitalContentRepository = userBookmarksCourseRepository;
        }

        public async Task CreateOrSetToDateBookmarkItem(
            Guid itemId,
            Guid userId,
            bool isUnBookmark,
            AnalyticLearnerBookmarkItemType itemType,
            DateTime time,
            SAM_UserHistory userHistory)
        {
            if (isUnBookmark)
            {
                var bookmarkItems = await _userBookmarksDigitalContentRepository
                    .GetAllListAsync(
                        x => x.CourseId == itemId &&
                             x.UserId == userId &&
                             x.ToDate == null);

                if (bookmarkItems.Any())
                {
                    foreach (var bookmarkItem in bookmarkItems)
                    {
                        bookmarkItem.ToDate = time;
                        bookmarkItem.ChangedBy = userId;
                    }

                    await _userBookmarksDigitalContentRepository.UpdateManyAsync(bookmarkItems);
                }
                else
                {
                    _logger.LogWarning($"Bookmark course {itemId} of user {userId} does not exist");
                }
            }
            else
            {
                if (userHistory == null)
                {
                    _logger.LogError("UserHistory cannot be null");
                    return;
                }

                await _userBookmarksDigitalContentRepository.InsertAsync(new Learner_UserBookmarksCourse
                {
                    UserId = userId,
                    UserHistoryId = userHistory.Id,
                    DepartmentId = userHistory.DepartmentId,
                    CourseId = itemId,
                    CreatedBy = userId,
                    FromDate = time,
                    ItemType = itemType.ToString()
                });
            }
        }
    }

    public class AnalyticLearnerBookmarkSpaceService : AnalyticsShareService, IAnalyticsLearnerService
    {
        private readonly ILogger<AnalyticLearnerBookmarkSpaceService> _logger;
        private readonly IRepository<Learner_UserBookmarksSpace> _userBookmarksSpaceRepository;

        public AnalyticLearnerBookmarkSpaceService(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserBookmarksSpace> userBookmarksSpaceRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticLearnerBookmarkSpaceService>();
            _userBookmarksSpaceRepository = userBookmarksSpaceRepository;
        }

        public async Task CreateOrSetToDateBookmarkItem(
            Guid itemId,
            Guid userId,
            bool isUnBookmark,
            AnalyticLearnerBookmarkItemType itemType,
            DateTime time,
            SAM_UserHistory userHistory)
        {
            if (isUnBookmark)
            {
                var bookmarkItems = await _userBookmarksSpaceRepository
                    .GetAllListAsync(
                        x => x.SpaceId == itemId &&
                             x.UserId == userId &&
                             x.ToDate == null);

                if (bookmarkItems.Any())
                {
                    foreach (var bookmarkItem in bookmarkItems)
                    {
                        bookmarkItem.ToDate = time;
                        bookmarkItem.ChangedBy = userId;
                    }

                    await _userBookmarksSpaceRepository.UpdateManyAsync(bookmarkItems);
                }
                else
                {
                    _logger.LogWarning($"Bookmark space {itemId} of user {userId} does not exist");
                }
            }
            else
            {
                if (userHistory == null)
                {
                    _logger.LogError("UserHistory cannot be null");
                    return;
                }

                await _userBookmarksSpaceRepository.InsertAsync(
                    new Learner_UserBookmarksSpace
                    {
                        UserId = userId,
                        UserHistoryId = userHistory.Id,
                        DepartmentId = userHistory.DepartmentId,
                        SpaceId = itemId,
                        CreatedBy = userId,
                        FromDate = time
                    });
            }
        }
    }

    public class AnalyticLearnerLearnerLearningPathSpaceService : AnalyticsShareService, IAnalyticsLearnerService
    {
        private readonly ILogger<AnalyticLearnerLearnerLearningPathSpaceService> _logger;

        private readonly IRepository<Learner_UserBookmarksLearnerLearningPath>
            _userBookmarksLearnerLearningPathRepository;

        public AnalyticLearnerLearnerLearningPathSpaceService(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserBookmarksLearnerLearningPath> userBookmarksLearnerLearningPathRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticLearnerLearnerLearningPathSpaceService>();
            _userBookmarksLearnerLearningPathRepository = userBookmarksLearnerLearningPathRepository;
        }

        public async Task CreateOrSetToDateBookmarkItem(
            Guid itemId,
            Guid userId,
            bool isUnBookmark,
            AnalyticLearnerBookmarkItemType itemType,
            DateTime time,
            SAM_UserHistory userHistory)
        {
            if (isUnBookmark)
            {
                var bookmarkItems = await _userBookmarksLearnerLearningPathRepository
                    .GetAllListAsync(
                        x => x.UserLearningPathId == itemId &&
                             x.UserId == userId &&
                             x.ToDate == null);

                if (bookmarkItems.Any())
                {
                    foreach (var bookmarkItem in bookmarkItems)
                    {
                        bookmarkItem.ToDate = time;
                        bookmarkItem.ChangedBy = userId;
                    }

                    await _userBookmarksLearnerLearningPathRepository.UpdateManyAsync(bookmarkItems);
                }
                else
                {
                    _logger.LogWarning($"Bookmark space {itemId} of user {userId} does not exist");
                }
            }
            else
            {
                if (userHistory == null)
                {
                    _logger.LogError("UserHistory cannot be null");
                    return;
                }

                await _userBookmarksLearnerLearningPathRepository.InsertAsync(
                    new Learner_UserBookmarksLearnerLearningPath
                    {
                        UserId = userId,
                        UserHistoryId = userHistory.Id,
                        DepartmentId = userHistory.DepartmentId,
                        UserLearningPathId = itemId,
                        CreatedBy = userId,
                        FromDate = time
                    });
            }
        }
    }

    public class AnalyticLearnerLMMLearningPathSpaceService : AnalyticsShareService, IAnalyticsLearnerService
    {
        private readonly ILogger<AnalyticLearnerLMMLearningPathSpaceService> _logger;
        private readonly IRepository<Learner_UserBookmarksLearningPath> _userBookmarksLearningPathRepository;

        public AnalyticLearnerLMMLearningPathSpaceService(
            ILoggerFactory loggerFactory,
            IRepository<Learner_UserBookmarksLearningPath> userBookmarksLearningPathRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticLearnerLMMLearningPathSpaceService>();
            _userBookmarksLearningPathRepository = userBookmarksLearningPathRepository;
        }

        public async Task CreateOrSetToDateBookmarkItem(
            Guid itemId,
            Guid userId,
            bool isUnBookmark,
            AnalyticLearnerBookmarkItemType itemType,
            DateTime time,
            SAM_UserHistory userHistory)
        {
            if (isUnBookmark)
            {
                var bookmarkItems = await _userBookmarksLearningPathRepository
                    .GetAllListAsync(
                        x => x.LearningPathId == itemId &&
                             x.UserId == userId &&
                             x.ToDate == null);

                if (bookmarkItems.Any())
                {
                    foreach (var bookmarkItem in bookmarkItems)
                    {
                        bookmarkItem.ToDate = time;
                        bookmarkItem.ChangedBy = userId;
                    }

                    await _userBookmarksLearningPathRepository.UpdateManyAsync(bookmarkItems);
                }
                else
                {
                    _logger.LogWarning($"Bookmark space {itemId} of user {userId} does not exist");
                }
            }
            else
            {
                if (userHistory == null)
                {
                    _logger.LogError("UserHistory cannot be null");
                    return;
                }

                await _userBookmarksLearningPathRepository.InsertAsync(
                    new Learner_UserBookmarksLearningPath
                    {
                        UserId = userId,
                        UserHistoryId = userHistory.Id,
                        DepartmentId = userHistory.DepartmentId,
                        LearningPathId = itemId,
                        CreatedBy = userId,
                        FromDate = time
                    });
            }
        }
    }

#pragma warning restore SA1402 // File may only contain a single type
}
