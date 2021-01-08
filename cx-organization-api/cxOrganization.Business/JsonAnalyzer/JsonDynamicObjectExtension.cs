using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Business.JsonAnalyzer
{
    public static class JsonObjectExtension
    {
        public static T GetValue<T>(this JObject data, string name, bool ignoreCase = true)
        {
            return data.GetValue(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).Value<T>();
        }

        public static List<dynamic> AsListDynamic(this object list)
        {
            return list as List<dynamic>;
        }

    }
}
