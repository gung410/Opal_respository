using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.DataAccess.Notification
{
    public class NotificationUpdateModel
    {
        public bool Active { get; set; }
        public string DefaultBody { get; set; }
        public string DefaultSubject { get; set; }
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }
        public bool IsGlobal { get; set; }
    }
}
