using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Domain.ValueObjects.Questions;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Models
{
    public class FormAssessmentModel
    {
        public FormAssessmentModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAssessmentModel(FormEntity formEntity, IEnumerable<FormQuestion> formQuestionEntities)
        {
            Id = formEntity.Id;
            Name = formEntity.Title;
            Type = formEntity.Type;
            Scales = formQuestionEntities
                    .Where(m => m.Question_Type == QuestionType.Scale)
                    .OrderBy(m => m.Priority)
                    .Select(m => new FormAssessmentScaleModel(m))
                    .ToList();
            Criteria = formQuestionEntities
                    .Where(m => m.Question_Type == QuestionType.Criteria)
                    .Select(m => new FormAssessmentCriteriaModel(m, Scales))
                    .ToList();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public FormType Type { get; set; }

        public IEnumerable<FormAssessmentScaleModel> Scales { get; set; }

        public IEnumerable<FormAssessmentCriteriaModel> Criteria { get; set; }
    }
}
