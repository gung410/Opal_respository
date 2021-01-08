using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microservice.Analytics.Application.Common.Converters
{
    /// <summary>
    /// This converter support to parse nullable to enum. For example, CourseUserGroupType is sometimes null on SAM module when User was created .
    /// We will return default Enum if the CourseUserGroupType is null.
    /// </summary>
    /// <typeparam name="T">Type of Enum.</typeparam>
    public sealed class NullableToEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var isNullable = IsNullableType(typeToConvert);
                var enumType = isNullable ? Nullable.GetUnderlyingType(typeToConvert) : typeToConvert;
                var names = Enum.GetNames(enumType ?? throw new InvalidOperationException());

                var enumText = System.Text.Encoding.UTF8.GetString(reader.ValueSpan);
                var match = names.FirstOrDefault(e => string.Equals(e, enumText, StringComparison.OrdinalIgnoreCase));
                return (T)Enum.Parse(enumType, match);
            }
            catch (Exception)
            {
                // This code support for purpose resolve problem happen on SAM.
                // Sometime, they don't send type in RabbitMQ message and it make QC to stop testing due to error.
                return default(T);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
