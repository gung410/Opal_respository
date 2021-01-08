using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Course.Application.Events.Todos
{
    public class OrderRefreshmentNotifyCAPayload : BaseTodoEventPayload
    {
        public string CourseName { get; set; }
    }
}
