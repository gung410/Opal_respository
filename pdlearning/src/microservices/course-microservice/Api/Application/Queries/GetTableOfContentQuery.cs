using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetTableOfContentQuery : BaseThunderQuery<List<ContentItem>>
    {
        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public bool IncludeAdditionalInfo { get; set; }

        public string SearchText { get; set; }
    }
}
