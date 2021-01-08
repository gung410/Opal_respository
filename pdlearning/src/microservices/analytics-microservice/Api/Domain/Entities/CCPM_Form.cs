using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_Form
    {
        public CCPM_Form()
        {
            CamCoursePostCourseEvaluationForm = new HashSet<CAM_Course>();
            CamCoursePreCourseEvaluationForm = new HashSet<CAM_Course>();
            CamLectureContent = new HashSet<CAM_LectureContent>();
            CcpmFormAnswer = new HashSet<CCPM_FormAnswer>();
            CcpmFormComment = new HashSet<CCPM_FormComment>();
            CcpmFormParticipant = new HashSet<CCPM_FormParticipant>();
            CcpmFormQuestion = new HashSet<CCPM_FormQuestion>();
        }

        public Guid FormId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public Guid? UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public Guid OwnerId { get; set; }

        public Guid? OwnerHistoryId { get; set; }

        public string OwnerDepartmentId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DueDate { get; set; }

        public int? InSecondTimeLimit { get; set; }

        public short? MaxAttempt { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public string SurveyType { get; set; }

        public short? PassingMarkPercentage { get; set; }

        public int? PassingMarkScore { get; set; }

        public string SqRatingType { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual ICollection<CAM_Course> CamCoursePostCourseEvaluationForm { get; set; }

        public virtual ICollection<CAM_Course> CamCoursePreCourseEvaluationForm { get; set; }

        public virtual ICollection<CAM_LectureContent> CamLectureContent { get; set; }

        public virtual ICollection<CCPM_FormAnswer> CcpmFormAnswer { get; set; }

        public virtual ICollection<CCPM_FormComment> CcpmFormComment { get; set; }

        public virtual ICollection<CCPM_FormParticipant> CcpmFormParticipant { get; set; }

        public virtual ICollection<CCPM_FormQuestion> CcpmFormQuestion { get; set; }
    }
}
