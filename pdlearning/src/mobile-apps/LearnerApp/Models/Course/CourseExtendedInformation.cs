using System;
using System.Collections.Generic;
using System.Globalization;
using LearnerApp.Common;

namespace LearnerApp.Models
{
    public class CourseExtendedInformation : CourseInformation
    {
        public int DurationHours { get; set; }

        public string LearningMode { get; set; }

        public string CourseObjective { get; set; }

        public List<string> CategoryIds { get; set; }

        public List<int> OwnerDivisionIds { get; set; }

        public List<int> OwnerBranchIds { get; set; }

        public List<int> PartnerOrganisationIds { get; set; }

        public string MoeOfficerId { get; set; }

        public string MoeOfficerPhoneNumber { get; set; }

        public string MoeOfficerEmail { get; set; }

        public double NotionalCost { get; set; }

        public double CourseFee { get; set; }

        public List<string> TrainingAgency { get; set; }

        public List<string> OtherTrainingAgencyReason { get; set; }

        public bool AllowPersonalDownload { get; set; }

        public bool AllowNonCommerInMOEReuseWithModification { get; set; }

        public bool AllowNonCommerReuseWithModification { get; set; }

        public bool AllowNonCommerInMoeReuseWithoutModification { get; set; }

        public bool AllowNonCommerReuseWithoutModification { get; set; }

        public bool IsLike { get; set; }

        public string CopyrightOwner { get; set; }

        public int NumOfSchoolLeader { get; set; }

        public int NumOfSeniorOrLeadTeacher { get; set; }

        public int NumOfMiddleManagement { get; set; }

        public int NumOfBeginningTeacher { get; set; }

        public int TotalLike { get; set; }

        public string PlaceOfWork { get; set; }

        public List<string> PrerequisiteCourseIds { get; set; }

        public List<string> ApplicableDivisionIds { get; set; }

        public List<string> ApplicableBranchIds { get; set; }

        public List<string> ApplicableZoneIds { get; set; }

        public List<string> ApplicableClusterIds { get; set; }

        public List<string> ApplicableSchoolIds { get; set; }

        public List<string> TrackIds { get; set; }

        public List<string> DevelopmentalRoleIds { get; set; }

        public List<string> TeachingLevels { get; set; }

        public List<string> TeachingSubjectIds { get; set; }

        public List<string> TeachingCourseStudyIds { get; set; }

        public List<string> CocurricularActivityIds { get; set; }

        public List<string> JobFamily { get; set; }

        public List<string> EasSubstantiveGradeBandingIds { get; set; }

        public string PdAreaThemeId { get; set; }

        public string PdAreaThemeCode { get; set; }

        public List<string> ServiceSchemeIds { get; set; }

        public List<string> SubjectAreaIds { get; set; }

        public List<string> LearningFrameworkIds { get; set; }

        public List<string> LearningDimensionIds { get; set; }

        public List<string> LearningAreaIds { get; set; }

        public List<string> LearningSubAreaIds { get; set; }

        public List<string> TeacherOutcomeIds { get; set; }

        public string NatureOfCourse { get; set; }

        public int NumOfPlannedClass { get; set; }

        public int NumOfSessionPerClass { get; set; }

        public int NumOfHoursPerSession { get; set; }

        public int MinParticipantPerClass { get; set; }

        public int MaxParticipantPerClass { get; set; }

        public long TotalHoursAttendWithinYear { get; set; }

        public DateTime PlanningPublishDate { get; set; }

        public DateTime PlanningArchiveDate { get; set; }

        public List<string> PdActivityPeriods { get; set; }

        public string FirstAdministratorId { get; set; }

        public string PrimaryApprovingOfficerId { get; set; }

        public List<string> CollaborativeContentCreatorIds { get; set; }

        public List<string> CourseFacilitatorIds { get; set; }

        public List<string> CourseCoFacilitatorIds { get; set; }

        public string ContentStatus { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public string ExternalCode { get; set; }

        public string CourseOutlineStructure { get; set; }

        public DateTime SubmittedDate { get; set; }

        public DateTime ApprovalDate { get; set; }

        public string AcknowledgementAndCredit { get; set; }

        public string Remarks { get; set; }

        public string PreCourseEvaluationFormId { get; set; }

        public string PostCourseEvaluationFormId { get; set; }

        public string ECertificateTemplateId { get; set; }

        public string ECertificatePrerequisite { get; set; }

        public string AlternativeApprovingOfficerId { get; set; }

        public RegistrationMethod? RegistrationMethod { get; set; }

        public int MaxReLearningTimes { get; set; }

        public int TotalView { get; set; }

        public int TotalShare { get; set; }

        public string GetCost()
        {
            string cost = "0";

            if (CourseFee == 0 && NotionalCost > 0)
            {
                cost = NotionalCost.ToString(CultureInfo.InvariantCulture);
            }
            else if (CourseFee > 0 && NotionalCost == 0)
            {
                cost = CourseFee.ToString(CultureInfo.InvariantCulture);
            }
            else if (CourseFee > 0 && NotionalCost > 0)
            {
                cost = NotionalCost.ToString(CultureInfo.InvariantCulture);
            }

            return $"{cost} SGD";
        }
    }
}
