using System;

namespace Microservice.Form.Application.Models
{
    public class NotifyFormDueDateModel
    {
        public Guid FormID { get; set; }

        public string FormName { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public int ReminderBeforeDays { get; set; }
    }
}
