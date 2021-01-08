using Microservice.LnaForm.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class TransferOwnershipCommand : BaseThunderCommand
    {
        public TransferOwnershipRequest Request { get; set; }
    }
}
