using cxOrganization.Domain.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.Exceptions;
using System.Reflection;
using System.Globalization;

namespace cxOrganization.Domain.Extensions
{
    public static class LineReaderExtension
    {
        public static string ReadFixLengthProperty(this string lineStr, ColumnInfo column)
        {
            var checkLength = column.StartIndex + column.Length;
            var limit = checkLength > lineStr.Length ? lineStr.Length - column.StartIndex : column.Length;
            if(column.StartIndex > lineStr.Length)
            {
                return null;
            }
            return lineStr.Substring(column.StartIndex, limit).Trim();
        }
        public static string ReadSplittedProperty(this string[] splittedLineStr, ColumnInfo column)
        {
            var columnIndex = column.Order - 1;
            var isIndexInRange = columnIndex >= 0 && columnIndex < splittedLineStr.Length;
            return isIndexInRange ? splittedLineStr[columnIndex].Trim() : null;
        }

        public static void SetColumnOrder(ILogger logger, PropertyMappingSetting mappingSetting, string[] headers)
        {
            var indexedHeaders = ToIndexedValues(headers);  // Get header Dic<caption,index>
            foreach (var columnInfo in mappingSetting.Columns) // loop config columns
            {
                if (indexedHeaders.TryGetValue(columnInfo.Caption, out var headerIndex))
                {
                    if (columnInfo.Order == headerIndex)
                    {
                        columnInfo.Order = headerIndex + 1; //columnInfo.Order  is started from 1
                    }
                }
                else
                {
                    logger.LogWarning($"Missing the column with caption '{columnInfo.Caption}' from file");
                    throw new UnsupportFileTemplateException($"Wrong the column order {headerIndex} of '{columnInfo.Caption}' from file");
                }

            }
        }

        public static void SetValueToProperty<T>(ILogger logger, PropertyInfo propertyInfo,
           ColumnInfo columnInfo, string defaultDateTimeFormat, string originalPropertyValue, T dataEntry,
           int lineIndex)
           where T : new()
        {
            if (originalPropertyValue == null) return;

            if (propertyInfo.PropertyType == typeof(DateTime)
                || propertyInfo.PropertyType == typeof(DateTime?))
            {
                var dateFormat = string.IsNullOrEmpty(columnInfo.Format)
                    ? defaultDateTimeFormat
                    : columnInfo.Format;
                if (TryParseDateValue(originalPropertyValue, dateFormat, out var dateValue))
                {
                    propertyInfo.SetValue(dataEntry, dateValue);
                }
                else
                {
                    logger.LogDebug(
                        $"Error when parsing '{originalPropertyValue}' to date value ({dateFormat}) for property {columnInfo.Name} at line {lineIndex}");
                }

                return;
            }


            if (propertyInfo.PropertyType.IsArray)
            {
                propertyInfo.SetValue(dataEntry,
                    originalPropertyValue.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim())
                        .ToArray());
                return;

            }
            if (propertyInfo.PropertyType == typeof(long) && long.TryParse(originalPropertyValue, out long digitPropertyValue))
            {
                propertyInfo.SetValue(dataEntry, digitPropertyValue);
                return;
            }
            propertyInfo.SetValue(dataEntry, originalPropertyValue);

        }

        private static Dictionary<string, int> ToIndexedValues(string[] values)
        {
            values = values.Distinct().ToArray();
            var indexOfValues = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];

                indexOfValues.Add(value, i);

            }
            return indexOfValues;
        }

        private static bool TryParseDateValue(string value, string dateTimePattern, out DateTime dateTime)
        {
            return DateTime.TryParseExact(value, dateTimePattern, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateTime);

        }
    }
}
