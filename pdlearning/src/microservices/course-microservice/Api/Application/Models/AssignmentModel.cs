using System;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class AssignmentModel
    {
        public AssignmentModel(Assignment entity, QuizAssignmentFormModel quizAssignmentFormModel = null)
        {
            Id = entity.Id;
            CourseId = entity.CourseId;
            ClassRunId = entity.ClassRunId;
            Title = entity.Title;
            Type = entity.Type;
            CreatedBy = entity.CreatedBy;
            ChangedBy = entity.ChangedBy;
            CreatedDate = entity.CreatedDate;
            DeletedDate = entity.DeletedDate;
            ChangedDate = entity.ChangedDate;
            QuizAssignmentForm = quizAssignmentFormModel;
            AssessmentConfig = entity.AssessmentConfig;
        }

        public Guid? Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public AssignmentType Type { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public QuizAssignmentFormModel QuizAssignmentForm { get; set; }

        public AssignmentAssessmentConfig AssessmentConfig { get; set; }
    }
}
