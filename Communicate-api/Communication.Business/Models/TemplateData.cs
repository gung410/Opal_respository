using System;
using System.Collections.Generic;

namespace Communication.Business.Models
{
    public class TemplateData
    {
        public TemplateData()
        {
            ReferenceData = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }
        public string Project { get; set; }
        public string Module { get; set; }
        public string TemplateName { get;  set; }
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, object> ReferenceData { get; set; }
 
    }
}