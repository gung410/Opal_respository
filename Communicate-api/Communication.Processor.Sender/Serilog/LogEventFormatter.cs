using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Processor.Sender.Serilog
{
    public class LogEventFormatter : ITextFormatter
    {

        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));
            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            var eventLogMessage = new EventLogMessage();
            eventLogMessage.Payload.Body.Message = message;
            eventLogMessage.Payload.Body.Timestamp = logEvent.Timestamp;
            eventLogMessage.Payload.Body.Level = logEvent.Level;
            eventLogMessage.Payload.Identity.ClientId = "Communication.Processor.Sender.com";
            eventLogMessage.Payload.Identity.CustomerId = "0";
            eventLogMessage.Payload.Identity.UserId = "0";
            if (logEvent.Exception != null)
            {
                eventLogMessage.Payload.Body.ErrorMessage = logEvent.Exception.Message;
                eventLogMessage.Payload.Body.ErrorSourse = logEvent.Exception.Source;
                eventLogMessage.Payload.Body.StackTrace = logEvent.Exception.StackTrace;
                if(logEvent.Exception.InnerException != null)
                {
                    eventLogMessage.Payload.Body.InnerException = new ExpandoObject();
                    eventLogMessage.Payload.Body.InnerException.ErrorMessage = logEvent.Exception.InnerException.Message;
                    eventLogMessage.Payload.Body.InnerException.StackTrace = logEvent.Exception.InnerException.StackTrace;
                    eventLogMessage.Payload.Body.ErrorSourse = logEvent.Exception.InnerException;
                }
            }
            if (logEvent.Properties.ContainsKey("CorrelationId"))
            {
                eventLogMessage.Payload.References.CorelationId = (logEvent.Properties["CorrelationId"] as ScalarValue)?.Value as string;
            }
            if (logEvent.Properties.ContainsKey("Entity"))
            {
                eventLogMessage.Routing.Entity = (logEvent.Properties["Entity"] as ScalarValue)?.Value as string;
            }
            else
            {
                eventLogMessage.Routing.Entity = Environment.MachineName;
            }
            if (logEvent.Properties.ContainsKey("EntityId"))
            {
                eventLogMessage.Routing.EntityId = (logEvent.Properties["EntityId"] as ScalarValue)?.Value as string;
            }
            else
            {
                eventLogMessage.Routing.EntityId = Environment.MachineName;
            }
            if (logEvent.Properties.ContainsKey("routing_prefix"))
            {
                eventLogMessage.Routing.Action = (logEvent.Properties["routing_prefix"] as ScalarValue)?.Value as string + "." + logEvent.Level;
            }
            if (logEvent.Properties.ContainsKey("SourceContext"))
            {
                eventLogMessage.Payload.Body.SourceContext = (logEvent.Properties["SourceContext"] as ScalarValue)?.Value as string;
            }
            var strings = eventLogMessage.ToJson();
            output.Write(strings);
        }
    }
}
