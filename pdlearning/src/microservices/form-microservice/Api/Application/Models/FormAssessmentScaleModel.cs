using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microservice.Form.Domain.Entities;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Models
{
    public class FormAssessmentScaleModel
    {
        public FormAssessmentScaleModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAssessmentScaleModel(FormQuestion formQuestionEntity)
        {
            Id = formQuestionEntity.Id;
            Name = formQuestionEntity.Title;
            Value = formQuestionEntity.Score;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public double? Value { get; set; }
    }
}
