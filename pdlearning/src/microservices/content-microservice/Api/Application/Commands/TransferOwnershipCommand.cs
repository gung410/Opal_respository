using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class TransferOwnershipCommand : BaseThunderCommand
    {
        public TransferOwnershipRequest Request { get; set; }
    }
}
