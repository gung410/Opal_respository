using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Domain.Entities;

namespace Microservice.Form.Application.Models
{
    public class QuestionGroupModel
    {
        public QuestionGroupModel()
        {
        }

        public QuestionGroupModel(QuestionGroup questionGroup)
        {
            Id = questionGroup.Id;
            Name = questionGroup.Name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
