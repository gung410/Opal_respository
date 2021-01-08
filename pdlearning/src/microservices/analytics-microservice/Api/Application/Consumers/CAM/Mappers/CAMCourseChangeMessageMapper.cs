using System;
using Microservice.Analytics.Application.Consumers.CAM.Messages;
using Microservice.Analytics.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.CAM.Mappers
{
    public static class CAMCourseChangeMessageMapper
    {
        public static CAM_CourseHistory MapToCAMCourseHistoryEntity(
            this CAMCourseChangeMessage message,
            int numOfHistory = 0,
            DateTime? deletedDate = null,
            Guid? cslSpaceId = null)
        {
            var camCourseHistory = new CAM_CourseHistory();
            if (message == null)
            {
                return camCourseHistory;
            }

            camCourseHistory.CourseId = message.Id;
            camCourseHistory.DeletedDate = deletedDate;
            camCourseHistory.CourseCode = message.CourseCode;
            camCourseHistory.CourseName = message.CourseName;
            camCourseHistory.CourseType = message.CourseType.ToString();
            camCourseHistory.CourseContent = message.CourseContent;
            camCourseHistory.IsDeleted = message.IsDeleted ?? false;
            camCourseHistory.CourseLevelId = message.CourseLevel;
            camCourseHistory.PostCourseEvaluationFormId = message.PostCourseEvaluationFormId;
            camCourseHistory.PreCourseEvaluationFormId = message.PreCourseEvaluationFormId;
            camCourseHistory.NatureOfCourseId = message.NatureOfCourse ?? Guid.Empty;
            camCourseHistory.Description = message.Description;
            camCourseHistory.CourseObjective = message.CourseObjective;
            camCourseHistory.DurationMinutes = message.DurationMinutes;
            camCourseHistory.Status = message.Status.ToString();
            camCourseHistory.StartDate = message.StartDate;
            camCourseHistory.PublishDate = message.PublishDate;
            camCourseHistory.CreatedByUserId = message.CreatedBy;
            camCourseHistory.ChangedByUserId = message.ChangedBy;
            camCourseHistory.ExpiredDate = message.ExpiredDate;
            camCourseHistory.Source = message.Source;
            camCourseHistory.AcknowledgementAndCredit = message.AcknowledgementAndCredit;
            camCourseHistory.Remarks = message.Remarks;
            camCourseHistory.CopyrightOwner = message.CopyrightOwner;
            camCourseHistory.AllowNonCommerInMoereuseWithModification = message.AllowNonCommerInMoereuseWithModification ?? false;
            camCourseHistory.AllowNonCommerInMoeReuseWithoutModification = message.AllowNonCommerInMoeReuseWithoutModification ?? false;
            camCourseHistory.AllowNonCommerReuseWithModification = message.AllowNonCommerReuseWithModification ?? false;
            camCourseHistory.AllowNonCommerReuseWithoutModification = message.AllowNonCommerReuseWithoutModification ?? false;
            camCourseHistory.AllowPersonalDownload = message.AllowPersonalDownload ?? false;
            camCourseHistory.AlternativeApprovingOfficerId = message.AlternativeApprovingOfficerId;
            camCourseHistory.ArchiveDate = message.ArchiveDate;
            camCourseHistory.CourseFee = message.CourseFee;
            camCourseHistory.CourseOutlineStructure = message.CourseOutlineStructure;
            camCourseHistory.DurationHours = message.DurationHours;
            camCourseHistory.FirstAdministratorId = message.FirstAdministratorId;
            camCourseHistory.LearningModeId = message.LearningMode;
            camCourseHistory.MaxParticipantPerClass = message.MaxParticipantPerClass;
            camCourseHistory.MaximumPlacesPerSchool = message.MaximumPlacesPerSchool;
            camCourseHistory.MoeofficerId = message.MoeofficerId;
            camCourseHistory.NotionalCost = message.NotionalCost;
            camCourseHistory.NumOfBeginningTeacher = message.NumOfBeginningTeacher;
            camCourseHistory.NumOfHoursPerClass = message.NumOfHoursPerClass;
            camCourseHistory.NumOfHoursPerSession = message.NumOfHoursPerSession;
            camCourseHistory.NumOfMiddleManagement = message.NumOfMiddleManagement;
            camCourseHistory.NumOfPlannedClass = message.NumOfPlannedClass;
            camCourseHistory.NumOfSchoolLeader = message.NumOfSchoolLeader;
            camCourseHistory.NumOfSeniorOrLeadTeacher = message.NumOfSeniorOrLeadTeacher;
            camCourseHistory.NumOfSessionPerClass = message.NumOfSessionPerClass;
            camCourseHistory.OtherTrainingAgencyReason = string.Join(';', message.OtherTrainingAgencyReason);
            camCourseHistory.EcertificatePrerequisite = message.ECertificatePrerequisite;
            camCourseHistory.EcertificateTemplateId = message.ECertificateTemplateId;
            camCourseHistory.PdactivityTypeId = message.PDActivityType;
            camCourseHistory.PdareaThemeCode = message.PDAreaThemeCode;
            camCourseHistory.PdareaThemeId = message.PDAreaThemeId;
            camCourseHistory.CoursePlanningCycleId = message.CoursePlanningCycleId;
            camCourseHistory.PlaceOfWork = message.PlaceOfWork;
            camCourseHistory.PrimaryApprovingOfficerId = message.PrimaryApprovingOfficerId;
            camCourseHistory.SecondAdministratorId = message.SecondAdministratorId;
            camCourseHistory.RegistrationMethod = message.RegistrationMethod;
            camCourseHistory.SubmittedDate = message.SubmittedDate;
            camCourseHistory.ExternalCode = message.ExternalCode;
            camCourseHistory.PlanningArchiveDate = message.PlanningArchiveDate;
            camCourseHistory.PlanningPublishDate = message.PlanningPublishDate;
            camCourseHistory.ContentStatus = message.ContentStatus.ToString();
            camCourseHistory.PublishedContentDate = message.PublishedContentDate;
            camCourseHistory.SubmittedContentDate = message.SubmittedContentDate;
            camCourseHistory.ApprovalContentDate = message.ApprovalContentDate;
            camCourseHistory.ApprovalDate = message.ApprovalDate;
            camCourseHistory.DepartmentId = message.DepartmentId.ToString();
            camCourseHistory.IsMigrated = message.IsMigrated ?? false;
            camCourseHistory.VerifiedDate = message.VerifiedDate;
            camCourseHistory.FullTextSearch = message.FullTextSearch;
            camCourseHistory.FullTextSearchKey = message.FullTextSearchKey;
            camCourseHistory.MaxReLearningTimes = message.MaxReLearningTimes;
            camCourseHistory.FromDate = Clock.Now;
            camCourseHistory.ToDate = null;
            camCourseHistory.No = numOfHistory;
            camCourseHistory.NumOfMinutesPerSession = message.NumOfMinutesPerSession;
            camCourseHistory.NumOfExperiencedTeacher = message.NumOfExperiencedTeacher;
            camCourseHistory.SpaceId = cslSpaceId;
            camCourseHistory.CourseNameInECertificate = message.CourseNameInECertificate;
            camCourseHistory.IsLearnerStarted = message.IsLearnerStarted;
            camCourseHistory.TotalHoursAttendWithinYear = message.TotalHoursAttendWithinYear;

            return camCourseHistory;
        }
    }
}
