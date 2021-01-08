using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_Section
    {
        public CAM_Section()
        {
            CamLecture = new HashSet<CAM_Lecture>();
        }

        public Guid SectionId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Order { get; set; }

        public int? CreditsAward { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual ICollection<CAM_Lecture> CamLecture { get; set; }
    }
}
