using System.Collections.Generic;

namespace LearnerApp.Models
{
    public class Resource
    {
        public string ResourceId { get; set; }

        public string ResourceType { get; set; }

        public string MainSubjectAreaTagId { get; set; }

        public string PreRequisties { get; set; }

        public string ObjectivesOutCome { get; set; }

        public string CreatedBy { get; set; }

        public List<string> Tags { get; set; }
    }
}
