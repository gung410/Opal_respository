using System;
using Microservice.Uploader.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Commands
{
    public class SavePersonalFileCommand : BaseThunderCommand
    {
        public CreatePersonalFilesRequest CreationRequest { get; set; }
    }
}
