using System;
using System.Threading.Tasks;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Services
{
    public class AnalyticsCSLLikeCommentService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLLikeCommentService> _logger;
        private readonly IRepository<CSL_LikeComment, int> _cslLikeCommentRepository;

        public AnalyticsCSLLikeCommentService(ILoggerFactory loggerFactory, IRepository<CSL_LikeComment, int> cslLikeCommentRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLLikeCommentService>();
            _cslLikeCommentRepository = cslLikeCommentRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var like = await _cslLikeCommentRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (like == null)
            {
                _logger.LogWarning($"Like comment {id} does not found");
                return;
            }

            like.ToDate = toDate ?? Clock.Now;
            await _cslLikeCommentRepository.UpdateAsync(like);
        }
    }

    public class AnalyticsCSLLikeForumService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLLikeForumService> _logger;
        private readonly IRepository<CSL_LikeForumThread, int> _cslLikeForumRepository;

        public AnalyticsCSLLikeForumService(ILoggerFactory loggerFactory, IRepository<CSL_LikeForumThread, int> cslLikeForumRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLLikeForumService>();
            _cslLikeForumRepository = cslLikeForumRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var like = await _cslLikeForumRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (like == null)
            {
                _logger.LogWarning($"Like forum {id} does not found");
                return;
            }

            like.ToDate = toDate ?? Clock.Now;
            await _cslLikeForumRepository.UpdateAsync(like);
        }
    }

    public class AnalyticsCSLLikePollService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLLikePollService> _logger;
        private readonly IRepository<CSL_LikePoll, int> _cslLikePollRepository;

        public AnalyticsCSLLikePollService(ILoggerFactory loggerFactory, IRepository<CSL_LikePoll, int> cslLikePollRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLLikePollService>();
            _cslLikePollRepository = cslLikePollRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var like = await _cslLikePollRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (like == null)
            {
                _logger.LogWarning($"Like poll {id} does not found");
                return;
            }

            like.ToDate = toDate ?? Clock.Now;
            await _cslLikePollRepository.UpdateAsync(like);
        }
    }

    public class AnalyticsCSLLikePostService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLLikePostService> _logger;
        private readonly IRepository<CSL_LikePost, int> _cslLikePostRepository;

        public AnalyticsCSLLikePostService(ILoggerFactory loggerFactory, IRepository<CSL_LikePost, int> cslLikePostRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLLikePostService>();
            _cslLikePostRepository = cslLikePostRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var like = await _cslLikePostRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (like == null)
            {
                _logger.LogWarning($"Like post {id} does not found");
                return;
            }

            like.ToDate = toDate ?? Clock.Now;
            await _cslLikePostRepository.UpdateAsync(like);
        }
    }

    public class AnalyticsCSLLikeWikiService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLLikePostService> _logger;
        private readonly IRepository<CSL_LikeWikiPage, int> _cslLikeWikiRepository;

        public AnalyticsCSLLikeWikiService(ILoggerFactory loggerFactory, IRepository<CSL_LikeWikiPage, int> cslLikeWikiRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLLikePostService>();
            _cslLikeWikiRepository = cslLikeWikiRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var like = await _cslLikeWikiRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (like == null)
            {
                _logger.LogWarning($"Like wiki {id} does not found");
                return;
            }

            like.ToDate = toDate ?? Clock.Now;
            await _cslLikeWikiRepository.UpdateAsync(like);
        }
    }
}
