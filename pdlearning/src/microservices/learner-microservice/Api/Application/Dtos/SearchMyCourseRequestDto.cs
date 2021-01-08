using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class SearchMyCourseRequestDto : PagedResultRequestDto, ISearchRequest<MyLearningStatus>
    {
        public string SearchText { get; set; }

        public bool IncludeStatistic { get; set; }

        public MyLearningStatus? StatusFilter { get; set; }

        public List<MyLearningStatus> StatisticsFilter { get; set; }

        public LearningCourseType CourseType { get; set; }
    }
}
