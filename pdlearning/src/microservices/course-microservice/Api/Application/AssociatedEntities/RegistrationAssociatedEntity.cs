using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AssociatedEntities
{
    public class RegistrationAssociatedEntity : Registration
    {
        public RegistrationAssociatedEntity()
        {
        }

        public RegistrationAssociatedEntity(Registration registration)
        {
            F.Copy(registration, this);
        }

        public RegistrationAssociatedEntity(Registration registration, CourseEntity course, ClassRun classRun)
        {
            F.Copy(registration, this);
            ClassRun = classRun;
            Course = course;
        }

        public ClassRun ClassRun { get; init; }

        public CourseEntity Course { get; init; }
    }
}
