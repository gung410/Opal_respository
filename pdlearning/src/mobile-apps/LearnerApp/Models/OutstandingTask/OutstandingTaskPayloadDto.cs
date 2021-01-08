using System.Collections.Generic;

namespace LearnerApp.Models.OutstandingTask
{
    public class OutstandingTaskPayloadDto
    {
        public int TotalCount { get; set; }

        public List<OutstandingTask> OutstandingTasks { get; set; }
    }
}
