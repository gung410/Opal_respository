using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Queries
{
    public class ValidateNominateLearnersQuery : BaseThunderQuery<List<ValidateNominateLearnerResultModel>>
    {
        public List<ValidateNominatedLearnersQueryRegistration> Registrations { get; set; }

        public List<Guid> GetRegistrationCourseIds()
        {
            return Registrations.Select(x => x.CourseId).Distinct().ToList();
        }
    }

    public class ValidateNominatedLearnersQueryRegistration
    {
        public Guid CourseId { get; set; }

        public Guid UserId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
