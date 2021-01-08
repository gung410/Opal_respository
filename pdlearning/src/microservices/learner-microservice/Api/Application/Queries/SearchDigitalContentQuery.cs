using System.Collections.Generic;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class SearchDigitalContentQuery : BaseThunderQuery<SearchPagedResultDto<DigitalContentModel, MyDigitalContentStatisticModel>>, IPagedResultAware
    {
        public string SearchText { get; set; }

        public MyDigitalContentStatus? StatusFilter { get; set; }

        public string OrderBy { get; set; }

        public List<DigitalContentType> DigitalContentType { get; set; }

        public bool IncludeStatistic { get; set; }

        public List<MyDigitalContentStatus> StatusStatistics { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
