using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microservice.Analytics.Application.Common.Converters
{
    /// <summary>
    /// This converter support to parse nullable to int. For example, UserId is sometime null on SAM module.
    /// We will return -1 if the UserId is null.
    /// </summary>
    public sealed class NullableToIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.GetInt32();
            }
            catch (Exception)
            {
                // Sometime, they don't send Id in RabbitMQ message and it make QC to stop testing due to error.
                return -1;
            }
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
