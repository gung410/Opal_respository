using System;
using System.Collections.Generic;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums.CourseCriteria;
using Thunder.Platform.Core.Extensions;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.RequestDtos
{
    public class SaveCourseCriteriaDto
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

        public IEnumerable<SaveCourseCriteriaDtoServiceScheme> CourseCriteriaServiceSchemes { get; set; }

        public CourseCriteriaPlaceOfWork PlaceOfWork { get; set; }

        public IEnumerable<Guid> PreRequisiteCourses { get; set; }

        public SaveCourseCriteriaCommand ToCommand()
        {
            return new SaveCourseCriteriaCommand()
            {
                Id = Id,
                AccountType = AccountType,
                Tracks = Tracks,
                DevRoles = DevRoles,
                TeachingLevels = TeachingLevels,
                TeachingCourseOfStudy = TeachingCourseOfStudy,
                JobFamily = JobFamily,
                CoCurricularActivity = CoCurricularActivity,
                SubGradeBanding = SubGradeBanding,
                CourseCriteriaServiceSchemes = CourseCriteriaServiceSchemes.SelectList(p => p.ToEntity()),
                PlaceOfWork = PlaceOfWork,
                PreRequisiteCourses = PreRequisiteCourses,
            };
        }
    }

    public class SaveCourseCriteriaDtoServiceScheme
    {
        public string ServiceSchemeId { get; set; }

        public int? MaxParticipant { get; set; }

        public CourseCriteriaServiceScheme ToEntity()
        {
            return new CourseCriteriaServiceScheme()
            {
                MaxParticipant = MaxParticipant,
                ServiceSchemeId = ServiceSchemeId
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
