using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public interface ISearchRequest<TEnum> where TEnum : struct
    {
        public string SearchText { get; set; }

        public bool IncludeStatistic { get; set; }

        public TEnum? StatusFilter { get; set; }

        public List<TEnum> StatisticsFilter { get; set; }
    }
}
