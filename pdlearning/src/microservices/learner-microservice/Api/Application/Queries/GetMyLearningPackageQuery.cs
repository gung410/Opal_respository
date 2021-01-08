using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyLearningPackageQuery : BaseThunderQuery<MyLearningPackageModel>
    {
        public Guid? MyLectureId { get; set; }

        public Guid? MyDigitalContentId { get; set; }
    }
}
