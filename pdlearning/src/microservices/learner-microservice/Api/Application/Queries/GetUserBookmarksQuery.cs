using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetUserBookmarksQuery : BaseThunderQuery<PagedResultDto<CourseModel>>, IPagedResultAware
    {
        public List<BookmarkType> BookmarkTypeFilter { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
