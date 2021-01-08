using System.Collections.Generic;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AssociatedEntities
{
    public class ClassRunAssociatedEntity : ClassRun
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassRunAssociatedEntity"/> class.
        /// Need to have default constructor for F.Copy.
        /// </summary>
        public ClassRunAssociatedEntity()
        {
        }

        public ClassRunAssociatedEntity(ClassRun classRun, CourseEntity course, IEnumerable<Session> sessions)
        {
            F.Copy(classRun, this);
            Course = course;
            Sessions = sessions;
        }

        public CourseEntity Course { get; set; }

        /// <summary>
        /// This is sessions of class. This property is nullable/optional load.
        /// </summary>
        public IEnumerable<Session> Sessions { get; set; }
    }
}
