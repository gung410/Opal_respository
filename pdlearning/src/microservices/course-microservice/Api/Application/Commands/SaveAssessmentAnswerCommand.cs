using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveAssessmentAnswerCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public IEnumerable<AssessmentCriteriaAnswer> CriteriaAnswers { get; set; }

        public bool IsSubmit { get; set; }
    }
}
