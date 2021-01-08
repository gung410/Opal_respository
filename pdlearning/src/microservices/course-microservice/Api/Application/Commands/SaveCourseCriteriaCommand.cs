using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums.CourseCriteria;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveCourseCriteriaCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public CourseCriteriaAccountType AccountType { get; set; }

        public IEnumerable<string> Tracks { get; set; }

        public IEnumerable<string> DevRoles { get; set; }

        public IEnumerable<string> TeachingLevels { get; set; }

        public IEnumerable<string> TeachingCourseOfStudy { get; set; }

        public IEnumerable<string> JobFamily { get; set; }

        public IEnumerable<string> CoCurricularActivity { get; set; }

        public IEnumerable<string> SubGradeBanding { get; set; }

        public IEnumerable<CourseCriteriaServiceScheme> CourseCriteriaServiceSchemes { get; set; }

        public CourseCriteriaPlaceOfWork PlaceOfWork { get; set; }

        public IEnumerable<Guid> PreRequisiteCourses { get; set; }
    }
}
