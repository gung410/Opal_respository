using System;

namespace Microservice.Analytics.Application.Consumers.SAM.Messages
{
    public class UserLoginMessage
    {
        public Guid UserId { get; set; }

        public bool LoginFromMobile { get; set; }

        public string ClientId { get; set; }

        public string SourceIp { get; set; }
    }
}
