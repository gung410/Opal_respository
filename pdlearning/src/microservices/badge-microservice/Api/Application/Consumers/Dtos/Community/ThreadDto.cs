using System;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class ThreadDto
    {
        public int Id { get; set; }

        public Guid CreatedBy { get; set; }

        public SourceType SourceType { get; set; }

        public SourceDto Source { get; set; }
    }
}
