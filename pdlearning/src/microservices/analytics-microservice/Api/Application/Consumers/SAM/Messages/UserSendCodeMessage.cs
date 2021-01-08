using System;

namespace Microservice.Analytics.Application.Consumers.SAM.Messages
{
    public class UserSendCodeMessage
    {
        public Guid UserId { get; set; }

        public string SelectedProvider { get; set; }
    }
}
