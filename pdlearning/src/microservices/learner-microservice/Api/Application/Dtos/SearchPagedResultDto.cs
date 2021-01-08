using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class SearchPagedResultDto<T, TStatistic> : PagedResultDto<T>
    {
        public SearchPagedResultDto()
        {
        }

        public SearchPagedResultDto(
            int totalCount,
            IReadOnlyList<T> items,
            List<TStatistic> statistics)
            : base(totalCount, items)
        {
            Statistics = statistics;
        }

        public List<TStatistic> Statistics { get; set; }
    }
}
