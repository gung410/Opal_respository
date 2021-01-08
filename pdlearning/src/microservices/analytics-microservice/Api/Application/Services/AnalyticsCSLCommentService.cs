using System;
using System.Threading.Tasks;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Services
{
    public class AnalyticsCSLCommentForumThreadService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly IRepository<CSL_CommentForumThread, int> _cslCommentForumRepository;
        private readonly ILogger<AnalyticsCSLCommentForumThreadService> _logger;

        public AnalyticsCSLCommentForumThreadService(IRepository<CSL_CommentForumThread, int> cslCommentForumRepository, ILoggerFactory loggerFactory)
        {
            _cslCommentForumRepository = cslCommentForumRepository;
            _logger = loggerFactory.CreateLogger<AnalyticsCSLCommentForumThreadService>();
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var comment = await _cslCommentForumRepository.FirstOrDefaultAsync(t => t.Id == id);

            if (comment == null)
            {
                _logger.LogWarning($"Comment Forum {id} does not found");
                return;
            }

            comment.ToDate = toDate ?? Clock.Now;
        }
    }

    public class AnalyticsCSLCommentPostService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly IRepository<CSL_CommentPost, int> _cslCommentPostRepository;
        private readonly ILogger<AnalyticsCSLCommentPostService> _logger;

        public AnalyticsCSLCommentPostService(IRepository<CSL_CommentPost, int> cslCommentPostRepository, ILoggerFactory loggerFactory)
        {
            _cslCommentPostRepository = cslCommentPostRepository;
            _logger = loggerFactory.CreateLogger<AnalyticsCSLCommentPostService>();
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var comment = await _cslCommentPostRepository.FirstOrDefaultAsync(t => t.Id == id);

            if (comment == null)
            {
                _logger.LogWarning($"Comment Post {id} does not found");
                return;
            }

            comment.ToDate = toDate ?? Clock.Now;
        }
    }

    public class AnalyticsCSLCommentPollService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly IRepository<CSL_CommentPoll, int> _cslCommentPollRepository;
        private readonly ILogger<AnalyticsCSLCommentPollService> _logger;

        public AnalyticsCSLCommentPollService(IRepository<CSL_CommentPoll, int> cslCommentPollRepository, ILoggerFactory loggerFactory)
        {
            _cslCommentPollRepository = cslCommentPollRepository;
            _logger = loggerFactory.CreateLogger<AnalyticsCSLCommentPollService>();
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var comment = await _cslCommentPollRepository.FirstOrDefaultAsync(t => t.Id == id);

            if (comment == null)
            {
                _logger.LogWarning($"Comment Poll {id} does not found");
                return;
            }

            comment.ToDate = toDate ?? Clock.Now;
        }
    }

    public class AnalyticsCSLCommentWikiService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly IRepository<CSL_CommentWikiPage, int> _cslCommentWikiRepository;
        private readonly ILogger<AnalyticsCSLCommentWikiService> _logger;

        public AnalyticsCSLCommentWikiService(IRepository<CSL_CommentWikiPage, int> cslCommentWikiRepository, ILoggerFactory loggerFactory)
        {
            _cslCommentWikiRepository = cslCommentWikiRepository;
            _logger = loggerFactory.CreateLogger<AnalyticsCSLCommentWikiService>();
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var comment = await _cslCommentWikiRepository.FirstOrDefaultAsync(t => t.Id == id);

            if (comment == null)
            {
                _logger.LogWarning($"Comment Wiki {id} does not found");
                return;
            }

            comment.ToDate = toDate ?? Clock.Now;
        }
    }
}
