using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Domain.Entities;

namespace Microservice.Form.Application.Models
{
    public class FormAssessmentCriteriaModel
    {
        public FormAssessmentCriteriaModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAssessmentCriteriaModel(FormQuestion formQuestionEntity, IEnumerable<FormAssessmentScaleModel> scaleColumns)
        {
            Id = formQuestionEntity.Id;
            Name = formQuestionEntity.Title;
            Scales = scaleColumns.Select(
                        column => new FormAssessmentCriteriaScaleModel(
                                    formQuestionEntity.Question_Options.FirstOrDefault(option => option.ScaleId == column.Id)));
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<FormAssessmentCriteriaScaleModel> Scales { get; set; }
    }
}
