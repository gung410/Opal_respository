using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LearnerApp.Services
{
    /// <inheritdoc />
    /// <summary>
    /// Defaults enum values to the base value if.
    /// </summary>
    public class DefaultEnumConverter : StringEnumConverter
    {
        /// <summary>
        /// The default value used to fallback on when a enum is not convertable.
        /// </summary>
        private readonly int defaultValue;

        public DefaultEnumConverter()
        {
        }

        public DefaultEnumConverter(int defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch
            {
                return Enum.Parse(objectType, $"{defaultValue}");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Validates that this converter can handle the type that is being provided.
        /// </summary>
        /// <param name="objectType">The type of the object being converted.</param>
        /// <returns>True if the base class says so, and if the value is an enum and has a default value to fall on.</returns>
        public override bool CanConvert(Type objectType)
        {
            return base.CanConvert(objectType) && objectType.GetTypeInfo().IsEnum && Enum.IsDefined(objectType, defaultValue);
        }
    }
}
