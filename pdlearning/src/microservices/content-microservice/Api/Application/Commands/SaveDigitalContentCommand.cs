using System;
using Microservice.Content.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class SaveDigitalContentCommand : BaseThunderCommand
    {
        public CreateDigitalContentRequest CreationRequest { get; set; }

        public UpdateDigitalContentRequest UpdateRequest { get; set; }

        public Guid UserId { get; set; }

        public int DepartmentId { get; set; }

        public bool IsCreation { get; set; }

        public Guid Id { get; set; }

        public bool IsSubmitForApproval { get; set; }
    }
}
