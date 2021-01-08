using System;
using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class GetUserReviewRequest : PagedResultRequestDto
    {
        public Guid ItemId { get; set; }

        public List<ItemType> ItemTypeFilter { get; set; }

        public string OrderBy { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
