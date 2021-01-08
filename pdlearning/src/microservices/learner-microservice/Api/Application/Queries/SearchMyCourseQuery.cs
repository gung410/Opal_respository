using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class SearchMyCourseQuery : BaseThunderQuery<SearchPagedResultDto<CourseModel, MyCourseStatisticModel>>, IPagedResultAware
    {
        public string SearchText { get; set; }

        public bool IncludeStatistic { get; set; }

        public LearningCourseType CourseType { get; set; }

        public MyLearningStatus? MyLearningStatusFilter { get; set; }

        public List<MyLearningStatus> MyLearningStatusStatistic { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
