using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetClassRunsByClassRunCodesQuery : BaseThunderQuery<List<ClassRunModel>>
    {
        public List<string> ClassRunCodes { get; set; }

        public Guid? CourseId { get; set; }
    }
}
