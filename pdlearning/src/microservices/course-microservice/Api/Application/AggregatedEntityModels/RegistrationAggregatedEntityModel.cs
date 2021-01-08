using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class RegistrationAggregatedEntityModel : BaseAggregatedEntityModel
    {
        public Registration Registration { get; private set; }

        public ClassRun ClassRun { get; private set; }

        public CourseEntity Course { get; private set; }

        public CourseUser User { get; private set; }

        public static RegistrationAggregatedEntityModel New()
        {
            return new RegistrationAggregatedEntityModel();
        }

        public RegistrationAggregatedEntityModel WithRegistration(Registration registration)
        {
            Registration = registration;

            return this;
        }

        public RegistrationAggregatedEntityModel WithClassRun(ClassRun classRun)
        {
            ClassRun = classRun;

            return this;
        }

        public RegistrationAggregatedEntityModel WithCourse(CourseEntity course)
        {
            Course = course;

            return this;
        }

        public RegistrationAggregatedEntityModel WithUser(CourseUser user)
        {
            User = user;

            return this;
        }
    }
}
