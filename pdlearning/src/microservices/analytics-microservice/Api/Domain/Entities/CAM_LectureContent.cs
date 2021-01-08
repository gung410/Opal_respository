using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_LectureContent
    {
        public Guid LectureContentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid LectureId { get; set; }

        public Guid? DigitalContentId { get; set; }

        public Guid? FormId { get; set; }

        public string MimeType { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public bool IsDeleted { get; set; }

        public string ExternalId { get; set; }

        public bool? QuizConfigByPassPassingRate { get; set; }

        public virtual CCPM_DigitalContent DigitalContent { get; set; }

        public virtual CCPM_Form Form { get; set; }

        public virtual CAM_Lecture Lecture { get; set; }
    }
}
