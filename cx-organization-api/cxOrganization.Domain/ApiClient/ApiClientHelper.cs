using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace cxOrganization.Domain.ApiClient
{
    public static class ApiClientHelper
    {
        /// <summary>
        /// Add list of optional filter to list of UriParam object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listItem"></param>
        /// <param name="name"></param>
        /// <param name="paramList"></param>
        public static void AddFiltersToUriParamList<T>(List<T> listItem, string name, List<UriParam> paramList)
        {
            if (listItem != null)
            {
                foreach (var item in listItem)
                {
                    paramList.Add(new UriParam() { Name = name, Value = item.ToString().Replace("&", "%26") });
                }
            }
        }

        /// <summary>
        /// Generate request uri base on list of UriParam
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="resourcePath"></param>
        /// <param name="paramList"></param>
        public static string GenerateRequestUri(string baseUri, string resourcePath, List<UriParam> paramList = null)
        {
            if (paramList != null && paramList.Any())
            {
                List<string> array = new List<string>();
                foreach (var param in paramList)
                {
                    if (string.IsNullOrEmpty(param.Name))
                    {
                        array.Add(string.Format("{0}", param.Value));
                    }
                    else
                    {
                        array.Add(string.Format("{0}={1}", param.Name, param.Value));
                    }

                }
                if (!resourcePath.EndsWith("?")) resourcePath = resourcePath + "?";

                resourcePath += string.Join("&", array);
            }

            return baseUri.EndsWith("/") || resourcePath.StartsWith("/") ? $"{baseUri}{resourcePath}" : $"{baseUri}/{resourcePath}";
        }

        /// <summary>
        /// Get header value by name 
        /// </summary>
        /// <param name="headerSrc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetHeaderValueByName(this HttpResponseHeaders headerSrc, string name)
        {
            IEnumerable<string> values;
            if (headerSrc.TryGetValues(name, out values))
                return values.FirstOrDefault();
            return null;
        }
    }

    /// <summary>
    /// This class holds name and value of optional filter on requestUri
    /// </summary>
    public class UriParam
    {
        /// <summary>
        /// Name of optional filter on requestUri
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// value of optional filter on requestUri
        /// </summary>
        public string Value { get; set; }
    }
}
