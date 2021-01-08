using System;
using System.Text.Json;

namespace Microservice.Form.Common.Helpers
{
    public static class F
    {
        public static T ParseObject<T>(object value)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value));
        }

        public static T DeepClone<T>(T value)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value));
        }

        public static TR Pipe<T, TR>(this T @this, Func<T, TR> func) => func(@this);
    }
}
