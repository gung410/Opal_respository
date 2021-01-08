using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetBlockoutDateDependenciesQuery : BaseThunderQuery<GetBlockoutDateDependenciesModel>
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<string> ServiceSchemes { get; set; }
    }
}
