using cxOrganization.WebServiceAPI.Validation;
using System.Collections.Generic;

namespace cxOrganization.WebServiceAPI.Models
{
    public class SearchPublicUserInfoParameters
    {
        [CollectionRequired]
        public List<string> UserCxIds { get; set; }
    }
}
