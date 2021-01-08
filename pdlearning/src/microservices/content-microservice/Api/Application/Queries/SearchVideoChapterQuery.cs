using System.Collections.Generic;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class SearchVideoChapterQuery : BaseThunderQuery<List<ChapterModel>>
    {
        public SearchVideoChapterRequest Request { get; set; }
    }
}
