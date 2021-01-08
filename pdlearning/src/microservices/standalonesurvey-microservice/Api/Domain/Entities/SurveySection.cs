using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    public class SurveySection : BaseEntity, ISoftDelete
    {
        public Guid SurveyId { get; set; }

        public string MainDescription { get; set; }

        public string AdditionalDescription { get; set; }

        public int Priority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
