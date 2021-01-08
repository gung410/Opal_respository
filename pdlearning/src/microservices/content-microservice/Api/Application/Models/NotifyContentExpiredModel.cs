using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Content.Application.Events;

namespace Microservice.Content.Application.Models
{
    public class NotifyContentExpiredModel<T>
    {
        public T Payload { get; set; }

        public Guid ContentCreatorId { get; set; }

        public Guid DigitalContentId { get; set; }

        public DateTime ExpiredDate { get; set; }
    }
}
