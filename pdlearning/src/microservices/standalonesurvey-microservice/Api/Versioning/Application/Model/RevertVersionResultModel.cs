using System;

namespace Microservice.StandaloneSurvey.Versioning.Application.Model
{
    public class RevertVersionResultModel
    {
        public bool IsSuccess { get; set; }

        public Guid UndoVersionId { get; set; }

        public Guid NewActiveId { get; set; }
    }
}
