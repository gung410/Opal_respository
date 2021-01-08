using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace cxOrganization.WebServiceAPI.DbMigration
{
    public static class DependencyHelper
    {
        public static void RegisterServices<T>(this IServiceCollection services,
            Assembly[] assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typeInfos = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
            if (typeInfos != null && typeInfos.Any())
            {
                foreach (var typeInfo in typeInfos)
                {
                    if (!typeInfo.IsAbstract)
                    {
                        services.Add(new ServiceDescriptor(typeof(T), typeInfo, lifetime));
                    }
                }
            }
        }
        public static T FindServiceByName<T>(string name, IEnumerable<T> services)
        {
            if (services == null || !services.Any())
                return default(T);
            return services.FirstOrDefault(t => t.GetServiceName() == name);
        }
        public static T FindServiceByName<T>(this IEnumerable<T> services, string name)
        {
            if (services == null || !services.Any())
                return default(T);
            return services.FirstOrDefault(t => t.GetServiceName() == name?.ToLower());
        }
        public static string GetServiceName(this object source)
        {
            var attributes = source.GetType().GetCustomAttributes(typeof(NameAttribute), true);
            var nameAttribute = attributes.FirstOrDefault();
            if (nameAttribute != null)
            {
                return ((NameAttribute)nameAttribute).Name.ToLower();
            }
            return null;
        }
    }
    public class NameAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NameAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public NameAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Dependency Injection Name
        /// </summary>
        public string Name { get; set; }
    }
}
