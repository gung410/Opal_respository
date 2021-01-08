using System;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CreateOrUpdateSectionCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public bool IsCreateNew { get; set; }

        public CreateOrUpdateSectionRequest CreateOrUpdateRequest { get; set; }
    }
}
