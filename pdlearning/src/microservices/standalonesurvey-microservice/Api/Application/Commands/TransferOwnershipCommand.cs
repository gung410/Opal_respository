using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class TransferOwnershipCommand : BaseStandaloneSurveyCommand
    {
        public TransferOwnershipRequest Request { get; set; }
    }
}
