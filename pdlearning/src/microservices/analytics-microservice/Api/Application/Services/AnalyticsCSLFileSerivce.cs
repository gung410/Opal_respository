using System;
using System.Threading.Tasks;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Services
{
    public class AnalyticsCSLFileCommentService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLFileCommentService> _logger;
        private readonly IRepository<CSL_FileComment, int> _cslFileCommentRepository;

        public AnalyticsCSLFileCommentService(ILoggerFactory loggerFactory, IRepository<CSL_FileComment, int> cslFileCommentRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLFileCommentService>();
            _cslFileCommentRepository = cslFileCommentRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var fileComment = await _cslFileCommentRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (fileComment == null)
            {
                _logger.LogWarning($"File comment {id} does not exist");
                return;
            }

            fileComment.ToDate = toDate ?? Clock.Now;
            await _cslFileCommentRepository.UpdateAsync(fileComment);
        }
    }

    public class AnalyticsCSLFileForumThreadService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLFileForumThreadService> _logger;
        private readonly IRepository<CSL_FileForumThread, int> _cslFileForumThreadRepository;

        public AnalyticsCSLFileForumThreadService(ILoggerFactory loggerFactory, IRepository<CSL_FileForumThread, int> cslFileForumThreadRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLFileForumThreadService>();
            _cslFileForumThreadRepository = cslFileForumThreadRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var fileComment = await _cslFileForumThreadRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (fileComment == null)
            {
                _logger.LogWarning($"File Forum thread {id} does not exist");
                return;
            }

            fileComment.ToDate = toDate ?? Clock.Now;
            await _cslFileForumThreadRepository.UpdateAsync(fileComment);
        }
    }

    public class AnalyticsCSLFilePollService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLFilePollService> _logger;
        private readonly IRepository<CSL_FilePoll, int> _cslFilePollRepository;

        public AnalyticsCSLFilePollService(ILoggerFactory loggerFactory, IRepository<CSL_FilePoll, int> cslFilePollRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLFilePollService>();
            _cslFilePollRepository = cslFilePollRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var fileComment = await _cslFilePollRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (fileComment == null)
            {
                _logger.LogWarning($"File poll {id} does not exist");
                return;
            }

            fileComment.ToDate = toDate ?? Clock.Now;
            await _cslFilePollRepository.UpdateAsync(fileComment);
        }
    }

    public class AnalyticsCSLFilePostService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLFilePostService> _logger;
        private readonly IRepository<CSL_FilePost, int> _cslFilePostRepository;

        public AnalyticsCSLFilePostService(ILoggerFactory loggerFactory, IRepository<CSL_FilePost, int> cslFilePostRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLFilePostService>();
            _cslFilePostRepository = cslFilePostRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var fileComment = await _cslFilePostRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (fileComment == null)
            {
                _logger.LogWarning($"File Post {id} does not exist");
                return;
            }

            fileComment.ToDate = toDate ?? Clock.Now;
            await _cslFilePostRepository.UpdateAsync(fileComment);
        }
    }

    public class AnalyticsCSLFileWikiPageService : AnalyticsShareService, IAnalyticsCSLService
    {
        private readonly ILogger<AnalyticsCSLFileWikiPageService> _logger;
        private readonly IRepository<CSL_FileWikiPage, int> _cslFileWikiPageRepository;

        public AnalyticsCSLFileWikiPageService(ILoggerFactory loggerFactory, IRepository<CSL_FileWikiPage, int> cslFileWikiPageRepository)
        {
            _logger = loggerFactory.CreateLogger<AnalyticsCSLFileWikiPageService>();
            _cslFileWikiPageRepository = cslFileWikiPageRepository;
        }

        public async Task SetToDateAsync(int id, DateTime? toDate = null)
        {
            var fileComment = await _cslFileWikiPageRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (fileComment == null)
            {
                _logger.LogWarning($"File WikiPage {id} does not exist");
                return;
            }

            fileComment.ToDate = toDate ?? Clock.Now;
            await _cslFileWikiPageRepository.UpdateAsync(fileComment);
        }
    }
}
