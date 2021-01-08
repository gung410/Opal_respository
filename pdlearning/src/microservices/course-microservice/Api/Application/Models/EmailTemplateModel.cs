using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Models
{
    public class EmailTemplateModel
    {
        public Guid Id { get; set; }

        public string Template { get; set; }

        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    }
}
