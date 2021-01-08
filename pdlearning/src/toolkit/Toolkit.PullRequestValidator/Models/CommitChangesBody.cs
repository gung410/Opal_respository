using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Models
{
    public class CommitChangesBody
    {
        [JsonProperty("changes")]
        public Collection<ChangeItems> Changes { get; set; }
    }
}
