using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Common
{
    /// <summary>
    /// A mapper to map from outstanding task type to priority.
    /// </summary>
    public static class TaskPriorityMapper
    {
        private static readonly IReadOnlyDictionary<OutstandingTaskType, int> TaskPriorityMap
            = new Dictionary<OutstandingTaskType, int>
            {
                { OutstandingTaskType.Course, 0 },
                { OutstandingTaskType.Assignment, 0 },
                { OutstandingTaskType.StandaloneForm, 1 },
                { OutstandingTaskType.Microlearning, 2 },
                { OutstandingTaskType.DigitalContent, 2 }
            };

        public static int MapFromTaskType(OutstandingTaskType type)
        {
            return TaskPriorityMap[type];
        }
    }
}
