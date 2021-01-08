using System;

namespace Microservice.Form.Application.RequestDtos
{
    public class GetFormStandaloneByIdRequestDto
    {
        public Guid FormOriginalObjectId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
