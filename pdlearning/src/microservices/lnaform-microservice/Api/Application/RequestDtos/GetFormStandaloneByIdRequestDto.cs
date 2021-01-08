using System;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class GetFormStandaloneByIdRequestDto
    {
        public Guid FormOriginalObjectId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
