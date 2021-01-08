using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Queries;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Services
{
    public class VideoChapterService : ApplicationService, IVideoChapterService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public VideoChapterService(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public async Task<List<ChapterModel>> CreateVideoChapter(CreateVideoChapterRequest request, Guid userId)
        {
            var saveCommand = new SaveVideoChapterCommand
            {
                CreationRequest = request,
                UserId = userId,
                SourceType = request.SourceType,
                IsCreation = true
            };
            await this._thunderCqrs.SendCommand(saveCommand);

            var searchQuery = new SearchVideoChapterQuery
            {
                Request = new SearchVideoChapterRequest
                {
                    ObjectId = request.ObjectId
                }
            };

            return await this._thunderCqrs.SendQuery(searchQuery);
        }

        public async Task<List<ChapterModel>> UpdateVideoChapter(UpdateVideoChapterRequest request, Guid userId)
        {
            var saveCommand = new SaveVideoChapterCommand
            {
                UpdateRequest = request,
                UserId = userId,
                SourceType = request.SourceType
            };
            await this._thunderCqrs.SendCommand(saveCommand);

            var searchQuery = new SearchVideoChapterQuery
            {
                Request = new SearchVideoChapterRequest
                {
                    ObjectId = request.ObjectId
                }
            };

            return await this._thunderCqrs.SendQuery(searchQuery);
        }

        public Task<List<ChapterModel>> SearchVideoChapters(SearchVideoChapterRequest dto, Guid userId)
        {
            var searchQuery = new SearchVideoChapterQuery
            {
                Request = dto
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }
    }
}
