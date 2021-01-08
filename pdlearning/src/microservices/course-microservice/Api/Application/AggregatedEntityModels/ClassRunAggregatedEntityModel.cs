using System.Collections.Generic;
using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class ClassRunAggregatedEntityModel : BaseAggregatedEntityModel<ClassRunAssociatedEntity>
    {
        private List<Session> _sessions;

        public ClassRun ClassRun { get; private set; }

        public CourseEntity Course { get; private set; }

        /// <summary>
        /// This property is optional.
        /// </summary>
        public List<Session> Sessions
        {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            get => _sessions ?? throw new GeneralException("Developer need to load with Sessions.");
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
            private set { _sessions = value; }
        }

        public static ClassRunAggregatedEntityModel Create(ClassRun classRun, CourseEntity course)
        {
            return new ClassRunAggregatedEntityModel()
            {
                ClassRun = classRun,
                Course = course
            };
        }

        public static ClassRunAggregatedEntityModel Create(ClassRun classRun, CourseEntity course, List<Session> sessions)
        {
            return new ClassRunAggregatedEntityModel()
            {
                ClassRun = classRun,
                Course = course,
                Sessions = sessions
            };
        }

        public override ClassRunAssociatedEntity ToAssociatedEntity()
        {
            if (_sessions == null)
            {
                throw new GeneralException("Developer need to load with sessions to map to associated entity");
            }

            return new ClassRunAssociatedEntity(ClassRun, Course, _sessions);
        }

        public ClassRunAggregatedEntityModel WithSessions(List<Session> sessions)
        {
            _sessions = sessions;
            return this;
        }
    }
}
