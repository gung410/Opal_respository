using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class SearchMyLearningPathRequestDto : PagedResultRequestDto, ISearchRequest<LearningPathType>
    {
        public string SearchText { get; set; }

        public bool IncludeStatistic { get; set; }

        public LearningPathType? StatusFilter { get; set; }

        public List<LearningPathType> StatisticsFilter { get; set; }
    }
}
