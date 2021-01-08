using System;
using LearnerApp.Models.StandaloneForm;

namespace LearnerApp.Models.OutstandingTask
{
    public class OutstandingTask
    {
        public string Id { get; set; }

        public string DigitalContentId { get; set; }

        public string CourseId { get; set; }

        public string FormId { get; set; }

        public OutstandingTaskStatusEum Status { get; set; }

        public string Name { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? StartDate { get; set; }

        public float Progress { get; set; }

        public OutstandingTaskTypeEnum Type { get; set; }

        public string FileExtension { get; set; }

        public string AssignmentId { get; set; }

        public StandaloneFormTypeEnum FormType { get; set; }
    }
}
