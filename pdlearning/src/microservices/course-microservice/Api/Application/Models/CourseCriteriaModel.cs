using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums.CourseCriteria;

namespace Microservice.Course.Application.Models
{
    public class CourseCriteriaModel
    {
        public CourseCriteriaModel()
        {
        }

        public CourseCriteriaModel(CourseCriteria courseCriteria)
        {
            Id = courseCriteria.Id;
            AccountType = courseCriteria.AccountType;
            Tracks = courseCriteria.Tracks;
            DevRoles = courseCriteria.DevRoles;
            TeachingLevels = courseCriteria.TeachingLevels;
            TeachingCourseOfStudy = courseCriteria.TeachingCourseOfStudy;
            JobFamily = courseCriteria.JobFamily;
            CoCurricularActivity = courseCriteria.CoCurricularActivity;
            SubGradeBanding = courseCriteria.SubGradeBanding;
            CourseCriteriaServiceSchemes = courseCriteria.CourseCriteriaServiceSchemes;
            PlaceOfWork = courseCriteria.PlaceOfWork;
            PreRequisiteCourses = courseCriteria.PreRequisiteCourses;
            CreatedBy = courseCriteria.CreatedBy;
            CreatedDate = courseCriteria.CreatedDate;
            ChangedBy = courseCriteria.ChangedBy;
            IsDeleted = courseCriteria.IsDeleted;
        }

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

        public Guid CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? ChangedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
