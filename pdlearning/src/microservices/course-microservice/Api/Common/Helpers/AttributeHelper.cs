using System.Collections.Generic;

namespace Microservice.Course.Common.Helpers
{
    public static class AttributeHelper
    {
        public static Dictionary<string, TAttribute> GetAttribute<T, TAttribute>()
            where TAttribute : System.Attribute
        {
            var dict = new Dictionary<string, TAttribute>();
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is TAttribute tAttribute)
                    {
                        dict.Add(prop.Name, tAttribute);
                    }
                }
            }

            return dict;
        }
    }
}
