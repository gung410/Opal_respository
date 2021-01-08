using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;

namespace cxOrganization.Domain.Services.ExportService
{
    public static class CsvExporter
    {
        public static byte[] ExportFromDataTables(ExportOption exportOption, List<DataTable> exportDataTables)
        {
            if (exportOption.SummaryPosition == SummaryPosition.Separated)
            {
                throw new NotSupportedException("Not support export to CSV with SummaryPosition is Separated");
            }
            else
            {
                return WriteDataTablesToSameCsvFile(exportOption, exportDataTables);
            }
        }

        public static byte[] ExportFromDataSets(ExportOption exportOption, List<DataSet> dataSets)
        {
            throw new NotSupportedException("Not support export to CSV with multiple files");
        }

        private static byte[] WriteDataTablesToMultipleCsvFile(ExportOption exportOption, List<DataTable> exportDataTables)
        {
            return new byte[0];
        }
        private static byte[] WriteDataTablesToSameCsvFile(ExportOption exportOption, List<DataTable> exportDataTables)
        {
            var csvString = new StringWriter();
            using (var csvWriter = new CsvWriter(csvString, CultureInfo.InvariantCulture, false))
            {
                csvWriter.Configuration.Delimiter = exportOption.CsvDelimiter;


                var summaryTables = exportDataTables.Where(t=>t.IsSummaryTable()).ToList();
                var mainTables = exportDataTables.Except(summaryTables).ToList();

                if (exportOption.SummaryPosition == SummaryPosition.Top)
                {
                    WriteDataTablesToCsvWriter(csvWriter, summaryTables);

                    csvWriter.WriteField(string.Empty);
                    csvWriter.NextRecord();

                    WriteDataTablesToCsvWriter(csvWriter, mainTables);

                }
                else
                {

                    WriteDataTablesToCsvWriter(csvWriter, mainTables);

                    csvWriter.WriteField(string.Empty);
                    csvWriter.NextRecord();

                    WriteDataTablesToCsvWriter(csvWriter, summaryTables);

                }

            }
            return Encoding.UTF8.GetBytes(csvString.ToString());
        }

        private static void WriteDataTablesToCsvWriter(CsvWriter csvWriter, List<DataTable> dataTables)
        {
            foreach (DataTable dataTable in dataTables)
            {
                WriteDataTableToCsvWriter(csvWriter, dataTable);
            }
        }

        private static void WriteDataTableToCsvWriter( CsvWriter csvWriter, DataTable dataTable)
        {
            if (!dataTable.ShouldHideHeader())
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    csvWriter.WriteField(column.Caption);
                }

                csvWriter.NextRecord();
            }

            var columnCount = dataTable.Columns.Count;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    var cellValue = GetCellValue(dataRow, columnIndex, out var isGroup);
                    if (isGroup)
                    {
                        var valueAsString = $"{cellValue}";
                        valueAsString = valueAsString.PadRight(200, '-');
                        csvWriter.WriteField(valueAsString);
                        break;
                    }

                    csvWriter.WriteField(cellValue);
                }

                csvWriter.NextRecord();
            }
        }

        private static object GetCellValue(DataRow row, int columnIndex, out bool isGroup)
        {
            var cellValue = row[columnIndex];
            isGroup = false;
            if (cellValue is CellInfo cellInfo)
            {
                isGroup = cellInfo.IsGroup;
                return cellInfo.Value;
            }

            return cellValue;
        }

    }
}
