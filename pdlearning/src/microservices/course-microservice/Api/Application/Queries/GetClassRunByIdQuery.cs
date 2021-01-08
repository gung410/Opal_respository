using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetClassRunByIdQuery : BaseThunderQuery<ClassRunModel>
    {
        public Guid Id { get; set; }

        public bool LoadHasLearnerStarted { get; set; }
    }
}
