using System;
using Microservice.LnaForm.Application.Services;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Commands
{
    public class SaveFormSectionCommand : BaseThunderCommand
    {
        public CreateFormSectionRequestDto CreationRequest { get; set; }

        public UpdateFormSectionRequestDto UpdateRequest { get; set; }

        public Guid UserId { get; set; }

        public bool IsCreation { get; set; }
    }
}
