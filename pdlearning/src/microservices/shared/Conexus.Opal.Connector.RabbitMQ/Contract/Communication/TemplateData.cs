using System.Collections.Generic;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class TemplateData
    {
        public string Module { get; set; }

        public string Project { get; set; }

        public string TemplateName { get; set; }

        public IDictionary<string, dynamic> ReferenceData { get; set; }

        public IDictionary<string, dynamic> Data { get; set; }
    }
}
