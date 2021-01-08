using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class DeleteFormCommand : BaseStandaloneSurveyCommand
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }
    }
}
