using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.Services
{
    public interface IVideoChapterService
    {
        Task<List<ChapterModel>> CreateVideoChapter(CreateVideoChapterRequest request, Guid userId);

        Task<List<ChapterModel>> UpdateVideoChapter(UpdateVideoChapterRequest request, Guid userId);

        Task<List<ChapterModel>> SearchVideoChapters(SearchVideoChapterRequest dto, Guid userId);
    }
}
