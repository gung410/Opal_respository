using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_DigitalContent
    {
        public CCPM_DigitalContent()
        {
            CamLectureContent = new HashSet<CAM_LectureContent>();
            CcpmDigitalContentComments = new HashSet<CCPM_DigitalContentComments>();
            LearnerUserDigitalContent = new HashSet<Learner_UserDigitalContent>();
            LearnerUserReviews = new HashSet<Learner_UserReview>();
        }

        public Guid DigitalContentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public Guid? UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid? OwnerHistoryId { get; set; }

        public string OwnerDepartmentId { get; set; }

        public string Discriminator { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public double AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public virtual ICollection<CAM_LectureContent> CamLectureContent { get; set; }

        public virtual ICollection<CCPM_DigitalContentComments> CcpmDigitalContentComments { get; set; }

        public virtual ICollection<Learner_UserDigitalContent> LearnerUserDigitalContent { get; set; }

        public virtual ICollection<Learner_UserReview> LearnerUserReviews { get; set; }
    }
}
