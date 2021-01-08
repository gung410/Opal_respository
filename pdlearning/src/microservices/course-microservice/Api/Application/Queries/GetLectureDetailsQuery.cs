using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetLectureDetailsQuery : BaseThunderQuery<LectureModel>
    {
        public Guid LectureId { get; set; }
    }
}
