using System;
using System.Linq;
using System.Reflection;

namespace Microservice.Analytics.Common.Helpers
{
    public static class CheckDifferentHelper
    {
        public static bool HasDifferent<T>(T oldItem, T newItem, Func<PropertyInfo, bool> predicate)
        {
            var properties = typeof(T)
                .GetProperties().Where(predicate);

            foreach (var property in properties)
            {
                var oOldValue = property.GetValue(oldItem);
                var oNewValue = property.GetValue(newItem);
                if (!Equals(oOldValue, oNewValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
