using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveAssessmentAnswerRequest
    {
        public Guid Id { get; set; }

        public IEnumerable<AssessmentCriteriaAnswer> CriteriaAnswers { get; set; }

        public bool IsSubmit { get; set; }
    }
}
