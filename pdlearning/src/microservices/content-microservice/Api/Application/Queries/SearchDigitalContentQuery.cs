using System;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class SearchDigitalContentQuery : BaseThunderQuery<PagedResultDto<SearchDigitalContentModel>>
    {
        public Guid UserId { get; set; }

        public SearchDigitalContentRequest Request { get; set; }
    }
}
