using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendOrderRefreshmentNotifySpecificUsersEvent : BaseTodoEvent<SendOrderRefreshmentNotifySpecificUsersPayload>
    {
        public SendOrderRefreshmentNotifySpecificUsersEvent(
            Guid createBy,
            SendOrderRefreshmentNotifySpecificUsersPayload payload,
            List<string> assignedToEmails,
            string subject,
            string message)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:send-order-refreshment:{Guid.NewGuid()}";
            Subject = subject;
            Payload = payload;
            AssignedToEmails = assignedToEmails;
            CreatedBy = createBy;
            Message = message;
        }

        public List<string> AssignedToEmails { get; }
    }
}
