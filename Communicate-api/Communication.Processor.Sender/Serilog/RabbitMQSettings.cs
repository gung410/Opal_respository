using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Processor.Sender.Serilog
{
    public class RabbitMQSettings
    {
        public string[] HostNames { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
