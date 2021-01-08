using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseCollaborativeContentCreator
    {
        public Guid CourseId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }
    }
}
