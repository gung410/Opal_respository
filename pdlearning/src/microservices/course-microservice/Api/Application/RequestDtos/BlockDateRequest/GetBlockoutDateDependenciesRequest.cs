using System;
using System.Collections.Generic;
using Microservice.Course.Application.Queries;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetBlockoutDateDependenciesRequest
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<string> ServiceSchemes { get; set; }

        public GetBlockoutDateDependenciesQuery ToQuery()
        {
            return new GetBlockoutDateDependenciesQuery()
            {
                ServiceSchemes = ServiceSchemes,
                ToDate = ToDate,
                FromDate = FromDate
            };
        }
    }
}
