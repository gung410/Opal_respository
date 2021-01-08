using System;

namespace Microservice.Content.Application.RequestDtos
{
    public class RenameDigitalContentRequest
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
    }
}
