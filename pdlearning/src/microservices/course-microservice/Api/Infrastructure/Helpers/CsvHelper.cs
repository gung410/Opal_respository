using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microservice.Course.Attributes;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using TableColumn = Microservice.Course.Attributes.TableColumnAttribute;

namespace Microservice.Course.Infrastructure.Helpers
{
    public static class CsvHelper
    {
        public static byte[] ExportData<T>(List<T> data)
        {
            var dict = AttributeHelper.GetAttribute<T, TableColumn>();
            var builder = new StringBuilder();

            // Add header row
            builder.AppendLine(
                string.Join(
                    ",",
                    dict.Values.Select(p => p.ColumnName)));

            // Add rows
            data.ForEach(dataItem =>
            {
                builder.AppendLine(string.Join(
                    ",",
                    dict.Keys.Select(p => dataItem.GetPropValue(p))));
            });

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public static byte[] ExportData<T>(List<T> data, List<KeyValuePair<string, Func<T, string>>> colNameToCellValueFnList)
        {
            var builder = new StringBuilder();

            // Add header row
            builder.AppendLine(string.Join(",", colNameToCellValueFnList.Select(p => p.Key)));

            // Add rows
            data.ForEach(dataItem =>
            {
                builder.AppendLine(string.Join(",", colNameToCellValueFnList.Select(p => p.Value(dataItem))));
            });

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public static List<T> ImportData<T>(Stream fileStream, bool isUseHeader = false, bool hasHeaderLine = true)
        {
            try
            {
                using var reader = new StreamReader(fileStream);
                var result = new List<T>();
                var rowHeadersDict = new Dictionary<string, int>();
                var isFirstLine = true;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine && hasHeaderLine)
                    {
                        if (isUseHeader)
                        {
                            rowHeadersDict = line.Split(',').Select((name, index) => new { name, index }).Where(x => !string.IsNullOrEmpty(x.name)).ToDictionary(p => p.name, p => p.index);
                        }

                        isFirstLine = false;
                        continue;
                    }

                    var data = CreateInstanceWithData<T>(line, rowHeadersDict, hasHeaderLine);
                    isFirstLine = false;
                    if (data != null)
                    {
                        result.Add(data);
                    }
                }

                return result;
            }
            finally
            {
                fileStream.Close();
            }
        }

        private static T CreateInstanceWithData<T>(string row, Dictionary<string, int> rowHeadersDict, bool hasHeaderRow)
        {
            List<string> rowValues;
            if (string.IsNullOrEmpty(row) || (rowValues = row.Split(',').ToList()).All(string.IsNullOrEmpty))
            {
                return default;
            }

            T o = Activator.CreateInstance<T>();
            var objectProperties = typeof(T).GetProperties();

            foreach (var property in objectProperties)
            {
                var attr = property.GetCustomAttribute<TableColumnAttribute>();
                if (attr == null)
                {
                    continue;
                }

                // if file doesn't have header row, helper will base on column order
                var indexColumn = hasHeaderRow && rowHeadersDict.ContainsKey(attr.ColumnName) ? rowHeadersDict[attr.ColumnName] : attr.Order;
                if (indexColumn >= rowValues.Count)
                {
                    continue;
                }

                var cellData = rowValues[indexColumn];
                var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                if (typeConverter.IsValid(rowValues[indexColumn]))
                {
                    property.SetValue(o, typeConverter.ConvertFromString(cellData));
                }
            }

            return o;
        }
    }
}
