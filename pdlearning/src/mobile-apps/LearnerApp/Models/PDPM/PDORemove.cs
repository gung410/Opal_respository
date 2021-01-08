using System.Collections.Generic;

namespace LearnerApp.Models.PDPM
{
    public class PDORemove
    {
        public List<PDORemoveIdentity> Identities { get; set; }

        public bool DeactivateAllVersion { get; set; } = true;
    }
}
