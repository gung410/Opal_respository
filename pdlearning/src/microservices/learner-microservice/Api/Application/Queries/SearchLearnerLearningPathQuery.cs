using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class SearchLearnerLearningPathQuery : BaseThunderQuery<SearchPagedResultDto<LearnerLearningPathModel, LearningPathStatisticModel>>, IPagedResultAware
    {
        public string SearchText { get; set; }

        public bool IncludeStatistic { get; set; }

        public LearningPathType? LearningPathType { get; set; }

        public List<LearningPathType> LearningPathStatistic { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
