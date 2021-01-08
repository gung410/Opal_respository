using System;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class SaveVideoChapterCommand : BaseThunderCommand
    {
        public CreateVideoChapterRequest CreationRequest { get; set; }

        public UpdateVideoChapterRequest UpdateRequest { get; set; }

        public Guid UserId { get; set; }

        public VideoSourceType SourceType { get; set; } = VideoSourceType.CSL;

        public bool IsCreation { get; set; }
    }
}
