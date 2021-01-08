using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Thunder.Platform.EntityFrameworkCore.Extensions
{
    public static class ConfigurationExtensions
    {
        public static PropertyBuilder<T> ConfigureForEnum<T>(this PropertyBuilder<T> enu) where T : struct, IConvertible
        {
            var extraLengthForSafe = 10;

            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var length = Enum.GetNames(typeof(T)).Select(_ => _.Length).Max() + extraLengthForSafe;

            var specificEnumToStringConverterType = typeof(EnumToStringConverter<>).MakeGenericType(typeof(T));
            var converterInstance = Activator.CreateInstance(specificEnumToStringConverterType, new object[] { null });

            return enu
                .HasConversion((ValueConverter)converterInstance)
                .HasMaxLength(length)
                .IsUnicode(false);
        }
    }
}
