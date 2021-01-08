using System;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    public class SurveyAnswer : BaseEntity, ISoftDelete
    {
        public Guid FormId { get; set; }

        public Guid? ResourceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }

        public short Attempt { get; set; } = 1;

        public SurveyAnswerFormMetaData SurveyAnswerFormMetaData { get; set; } = new SurveyAnswerFormMetaData();

        public Guid OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsCompleted { get; set; }
    }
}
