using System;

namespace Microservice.Content.Application.Models
{
    public class DigitalContentExpiryInfoModel
    {
        public Guid Id { get; set; }

        public DateTime? ExpiredDate { get; set; }
    }
}
