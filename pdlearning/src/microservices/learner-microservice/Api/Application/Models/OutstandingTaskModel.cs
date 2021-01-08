using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class OutstandingTaskModel
    {
        public OutstandingTaskModel(string name)
        {
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; private set; }

        public float? Progress { get; private set; }

        public DateTime? DueDate { get; private set; }

        public DateTime? StartDate { get; private set; }

        public OutstandingTaskType Type { get; private set; }

        public OutstandingTaskStatus Status { get; private set; }

        public FileExtensionType? FileExtension { get; private set; }

        public FormType? FormType { get; private set; }

        public Guid? FormId { get; private set; }

        public Guid? CourseId { get; private set; }

        public Guid? AssignmentId { get; private set; }

        public Guid? DigitalContentId { get; private set; }

        public static OutstandingTaskModel New(string name)
        {
            return new OutstandingTaskModel(name);
        }

        public OutstandingTaskModel WithId(Guid? id)
        {
            if (id.HasValue)
            {
                Id = id.Value;
            }

            return this;
        }

        public OutstandingTaskModel WithStatus(OutstandingTaskStatus status)
        {
            Status = status;
            return this;
        }

        public OutstandingTaskModel WithTaskProgress(float? progress)
        {
            if (progress != null)
            {
                Progress = progress;
            }

            return this;
        }

        public OutstandingTaskModel WithEndDate(DateTime? endDate)
        {
            if (endDate != null)
            {
                DueDate = endDate;
            }

            return this;
        }

        public OutstandingTaskModel WithStartDate(DateTime? startDate)
        {
            if (startDate != null)
            {
                StartDate = startDate;
            }

            return this;
        }

        public OutstandingTaskModel WithFileExtension(FileExtensionType? fileExtension)
        {
            if (fileExtension != null)
            {
                FileExtension = fileExtension;
            }

            return this;
        }

        public OutstandingTaskModel WithCourseId(Guid? courseId)
        {
            if (courseId != null)
            {
                CourseId = courseId;
            }

            return this;
        }

        public OutstandingTaskModel WithDigitalContentId(Guid? digitalContentId)
        {
            if (digitalContentId != null)
            {
                DigitalContentId = digitalContentId;
            }

            return this;
        }

        public OutstandingTaskModel WithFormId(Guid? formId)
        {
            if (formId != null)
            {
                FormId = formId;
            }

            return this;
        }

        public OutstandingTaskModel WithFormType(FormType? formType)
        {
            if (formType != null)
            {
                FormType = formType;
            }

            return this;
        }

        public OutstandingTaskModel WithAssignmentId(Guid? assignmentId)
        {
            if (assignmentId != null)
            {
                AssignmentId = assignmentId;
            }

            return this;
        }

        public OutstandingTaskModel WithTaskType(OutstandingTaskType type)
        {
            Type = type;
            return this;
        }
    }
}
