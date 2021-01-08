using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeClassRunCancellationStatusRequest
    {
        public List<Guid> Ids { get; set; }

        public string Comment { get; set; }

        public ClassRunCancellationStatus CancellationStatus { get; set; }
    }
}
