using Communication.Business.Models;
using Communication.Business.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Communication.Business.Models.FirebaseCloudMessage
{
    public class NotificationMessageBody : MessageBody
    {
        public string NotificationId { get; set; }
        public ISet<string> RegistrationTokens { get; set; }
    }

}