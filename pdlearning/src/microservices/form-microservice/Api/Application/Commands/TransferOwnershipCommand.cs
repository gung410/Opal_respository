using Microservice.Form.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class TransferOwnershipCommand : BaseThunderCommand
    {
        public TransferOwnershipRequest Request { get; set; }
    }
}
