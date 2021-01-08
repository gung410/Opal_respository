using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class SearchMyDigitalContentRequestDto : PagedResultRequestDto, ISearchRequest<MyDigitalContentStatus>
    {
        public string SearchText { get; set; }

        public bool IncludeStatistic { get; set; }

        public MyDigitalContentStatus? StatusFilter { get; set; }

        public List<MyDigitalContentStatus> StatisticsFilter { get; set; }

        public string OrderBy { get; set; }

        public List<DigitalContentType> DigitalContentType { get; set; }
    }
}
