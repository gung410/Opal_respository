using CsvHelper;
using CsvHelper.Configuration;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Exceptions;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace cxOrganization.Domain.Services
{
    public class CsvFileReader : IDataFileReader
    {
        private readonly ILogger _logger;
        public CsvFileReader(ILogger<CsvFileReader> logger)
        {
            _logger = logger;
        }
        public FileType FileType => FileType.Csv;

        public List<T> ReadDataFromStream<T>(Stream stream, PropertyMappingSetting mappingSetting) where T : new()
        {
            switch (mappingSetting.ColumnType)
            {
                case ColumnType.FixLength:
                    return ReadData<T>(stream, mappingSetting, false, (columnInfo, readingCxt) => readingCxt.RawRecord.ReadFixLengthProperty(columnInfo));
                case ColumnType.Delimiter:
                    return ReadData<T>(stream, mappingSetting, false, (columnInfo, readingCxt) => readingCxt.Record.ReadSplittedProperty(columnInfo));
                case ColumnType.DelimiterWithCaption:
                    return ReadData<T>(stream, mappingSetting, true, (columnInfo, readingCxt) => readingCxt.Record.ReadSplittedProperty(columnInfo));
            }

            _logger.LogWarning($"Do not support read data with file type {FileType}, column type {mappingSetting.ColumnType}");
            return new List<T>();
        }

        private List<T> ReadData<T>(
            Stream stream, 
            PropertyMappingSetting mappingSetting, 
            bool indicatePropertyByHeader, 
            Func<ColumnInfo, ReadingContext, string> readFieldValue
        ) where T: new()
        {
            stream.ResetPosition();
            var data = new List<T>();

            var type = typeof(T);
            var propertyInfos = type.GetProperties().ToDictionary(p => p.Name);
            var columnInfos = mappingSetting.Columns;
            if(columnInfos == null || columnInfos.Count == 0)
            {
                _logger.LogWarning($"There is no configuration of column mapping to be able to map data");
                return data;
            }
            var defaultDateTimeFormat = !string.IsNullOrEmpty(mappingSetting.DateFormat)
               ? mappingSetting.DateFormat
               : CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern;

            using (var reader = new StreamReader(stream, mappingSetting.GetEncoding()))
            {
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.Configuration.Delimiter = mappingSetting.Delimiter;

                    var lineIndex = mappingSetting.SkipStartLine;
                    while (csv.Read())
                    {
                        if (lineIndex == 0)
                        {
                            List<string> headers = new List<string>();
                            foreach (var columnInfo in columnInfos)
                            {
                                if (!propertyInfos.TryGetValue(columnInfo.Name, out var propertyInfo))
                                {
                                    _logger.LogWarning($"There is no property with name '{columnInfo.Name}' in {type.Name}");
                                    continue;
                                }

                                var originalPropertyValue = readFieldValue(columnInfo, csv.Context);
                                if (string.IsNullOrEmpty(originalPropertyValue) || !originalPropertyValue.EqualsIgnoreCase(columnInfo.Caption))
                                {
                                    throw new UnsupportFileTemplateException($"Can not read {columnInfo.Caption} on file");
                                }
                                headers.Add(originalPropertyValue);
                            }
                            //checking less or more fields appear in file
                            if (csv.Context.Record.Length != headers.Count || csv.Context.Record.Length != columnInfos.Count)
                            {
                                throw new UnsupportFileTemplateException($"Some columns are not allowed to be less or more on file");
                            }

                            LineReaderExtension.SetColumnOrder(_logger, mappingSetting, headers.ToArray());
                        }
                        else
                        {
                            var dataEntry = new T();
                            foreach (var columnInfo in columnInfos)
                            {
                                if (!propertyInfos.TryGetValue(columnInfo.Name, out var propertyInfo))
                                {
                                    _logger.LogWarning($"There is no property with name '{columnInfo.Name}' in {type.Name}");
                                    continue;
                                }

                                var originalPropertyValue = readFieldValue(columnInfo, csv.Context);

                                LineReaderExtension.SetValueToProperty(_logger, propertyInfo, columnInfo,
                                    defaultDateTimeFormat, originalPropertyValue, dataEntry, lineIndex);

                            }

                            if (dataEntry is IInputRecord record)
                            {
                                record.RawRecordIndex = lineIndex;
                                record.RawRecord = csv.Context.RawRecord;
                            }

                            data.Add(dataEntry);

                        }
                        lineIndex++;
                    }
                }
            }

            return data;
        }
    }
    public interface IInputRecord
    {
        int RawRecordIndex { get; set; }
        object RawRecord { get; set; }
    }
}
