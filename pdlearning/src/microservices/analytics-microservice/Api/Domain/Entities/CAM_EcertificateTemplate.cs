using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_EcertificateTemplate
    {
        public CAM_EcertificateTemplate()
        {
            CamCourse = new HashSet<CAM_Course>();
        }

        public Guid EcertificateTemplateId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string Title { get; set; }

        public string PhysicalFileName { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<CAM_Course> CamCourse { get; set; }
    }
}
