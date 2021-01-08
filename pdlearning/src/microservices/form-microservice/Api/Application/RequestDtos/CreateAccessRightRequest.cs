using System;
using System.Collections.Generic;

namespace Microservice.Form.Application.RequestDtos
{
    public class CreateAccessRightRequest
    {
        public Guid? Id { get; set; }

        public IEnumerable<Guid> UserIds { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
