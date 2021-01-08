using System;
using System.Collections.Generic;

namespace Microservice.Content.Application.RequestDtos
{
    public class CreateAccessRightRequest
    {
        public Guid? Id { get; set; }

        public List<Guid> UserIds { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
