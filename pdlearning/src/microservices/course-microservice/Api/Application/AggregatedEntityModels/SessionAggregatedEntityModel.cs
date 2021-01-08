using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class SessionAggregatedEntityModel : BaseAggregatedEntityModel<SessionAssociatedEntity>
    {
        public Session Session { get; private set; }

        public ClassRun ClassRun { get; private set; }

        public CourseEntity Course { get; private set; }

        public static SessionAggregatedEntityModel Create(
            Session session, ClassRun classRun, CourseEntity course)
        {
            return new SessionAggregatedEntityModel()
            {
                Session = session,
                ClassRun = classRun,
                Course = course
            };
        }

        public override SessionAssociatedEntity ToAssociatedEntity()
        {
            return new SessionAssociatedEntity(Session, ClassRun, Course);
        }
    }
}
