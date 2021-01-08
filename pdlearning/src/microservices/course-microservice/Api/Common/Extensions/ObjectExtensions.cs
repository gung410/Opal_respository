using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Microservice.Course.Common.Extensions
{
    public static class ObjectExtensions
    {
        [return: MaybeNull]
        [CanBeNull]
        public static object GetPropValue([MaybeNull]this object obj, string propName)
        {
            return obj?.GetType().GetProperty(propName)?.GetValue(obj, null);
        }
    }
}
