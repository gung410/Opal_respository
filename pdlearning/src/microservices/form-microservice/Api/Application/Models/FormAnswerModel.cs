using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Domain.Entities;

namespace Microservice.Form.Application.Models
{
    public class FormAnswerModel
    {
        public FormAnswerModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAnswerModel(FormAnswer formAnswer, IEnumerable<FormQuestionAnswerModel> formQuestionAnswers)
        {
            Id = formAnswer.Id;
            FormId = formAnswer.FormId;
            CourseId = formAnswer.CourseId;
            MyCourseId = formAnswer.MyCourseId;
            ClassRunId = formAnswer.ClassRunId;
            AssignmentId = formAnswer.AssignmentId;
            StartDate = formAnswer.StartDate;
            EndDate = formAnswer.EndDate;
            SubmitDate = formAnswer.SubmitDate;
            Score = formAnswer.Score;
            ScorePercentage = formAnswer.ScorePercentage;
            Attempt = formAnswer.Attempt;
            FormMetaData = new FormAnswerFormMetaDataModel(formAnswer.FormMetaData);
            CreatedBy = formAnswer.CreatedBy;
            QuestionAnswers = formQuestionAnswers.ToList();
            IsCompleted = formAnswer.IsCompleted;
            CreatedDate = formAnswer.CreatedDate;
            ChangedDate = formAnswer.ChangedDate;
        }

        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? MyCourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? AssignmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }

        public double? Score { get; set; }

        public double? ScorePercentage { get; set; }

        public short Attempt { get; set; } = 1;

        public FormAnswerFormMetaDataModel FormMetaData { get; set; }

        public Guid CreatedBy { get; set; }

        public ICollection<FormQuestionAnswerModel> QuestionAnswers { get; set; } = new List<FormQuestionAnswerModel>();

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
