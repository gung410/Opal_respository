using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAllLectureNamesByListIdsQuery : BaseThunderQuery<LectureIdMapNameModel[]>
    {
        public Guid[] ListLectureIds { get; set; }
    }
}
