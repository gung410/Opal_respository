using System;
using System.Collections.Generic;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Application.Models
{
    public class FormAssessmentCriteriaScaleModel
    {
        public FormAssessmentCriteriaScaleModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAssessmentCriteriaScaleModel(QuestionOption formQuestionOptionEntity)
        {
            ScaleId = formQuestionOptionEntity.ScaleId.Value;
            Content = formQuestionOptionEntity.Value.ToString();
        }

        public Guid ScaleId { get; set; }

        public string Content { get; set; }
    }
}
