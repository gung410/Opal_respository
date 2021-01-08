using System.Collections.Generic;

namespace LearnerApp.Models.PDPM
{
    public class PDORemoveResponse
    {
        public List<object> OtherVersions { get; set; }

        public PDORemoveIdentity Identity { get; set; }

        public int Status { get; set; }
    }
}
