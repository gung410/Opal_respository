using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class PagedReviewDto<T> : PagedResultDto<T>
    {
        public PagedReviewDto()
        {
        }

        public PagedReviewDto(int totalCount, IReadOnlyList<T> items, double rating) : base(totalCount, items)
        {
            Rating = rating;
        }

        public double Rating { get; set; }
    }
}
