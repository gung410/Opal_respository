using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Conexus.Opal.Microservice.Metadata.Common
{
    public static class ObjectHelper
    {
        public static T ParseObject<T>(object value)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, options), options);
        }

        public static object ParseObject(object value, Type returnType)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize(JsonSerializer.Serialize(value, options), returnType, options);
        }
    }
}
