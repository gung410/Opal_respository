using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetUserReviewsQuery : BaseThunderQuery<PagedReviewDto<UserReviewModel>>, IPagedResultAware
    {
        public Guid ItemId { get; set; }

        public List<ItemType> ItemTypeFilter { get; set; }

        public string OrderBy { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
