using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AssociatedEntities
{
    public class SessionAssociatedEntity : Session
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionAssociatedEntity"/> class.
        /// Need to have default constructor for F.Copy.
        /// </summary>
        public SessionAssociatedEntity()
        {
        }

        public SessionAssociatedEntity(Session session, ClassRun classRun, CourseEntity course)
        {
            F.Copy(session, this);
            ClassRun = classRun;
            Course = course;
        }

        public ClassRun ClassRun { get; set; }

        public CourseEntity Course { get; set; }
    }
}
