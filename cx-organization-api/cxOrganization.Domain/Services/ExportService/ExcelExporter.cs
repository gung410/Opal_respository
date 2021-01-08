using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace cxOrganization.Domain.Services.ExportService
{
    public static class ExcelExporter
    {

        public static byte[] ExportFromDataTables(ExportOption exportOption, List<DataTable> exportDataTables)
        {
            if (exportOption.SummaryPosition == SummaryPosition.Separated)
            {
                return WriteDataTablesToMultipleExcelSheet(exportOption, exportDataTables);
            }
            else
            {
                return WriteDataTablesToSameExcelSheet(exportOption, exportDataTables);
            }
        }
        public static byte[] ExportFromDataSets(ExportOption exportOption, List<DataSet> dataSets)
        {
            using (var memory = new MemoryStream())
            {
                var workbook = new XSSFWorkbook();

                foreach (var dataSet in dataSets)
                {
                    var summaryTables = new List<DataTable>();
                    var mainTables = new List<DataTable>();
                    foreach (DataTable dataSetTable in dataSet.Tables)
                    {
                        if (dataSetTable.IsSummaryTable())
                            summaryTables.Add(dataSetTable);
                        else mainTables.Add(dataSetTable);
                    }

                    var excelSheet = CreateSheet(workbook, GetSheetName(dataSet));

                    if (exportOption.SummaryPosition == SummaryPosition.Top)
                    {
                        var nextIndex = WriteDataTablesToExcelSheet(workbook, excelSheet, summaryTables, 0);
                        excelSheet.CreateRow(nextIndex++);
                        WriteDataTablesToExcelSheet(workbook, excelSheet, mainTables, nextIndex);

                    }
                    else
                    {
                        var nextIndex = WriteDataTablesToExcelSheet(workbook, excelSheet, mainTables, 0);
                        excelSheet.CreateRow(nextIndex++);
                        WriteDataTablesToExcelSheet(workbook, excelSheet, summaryTables, nextIndex);

                    }

                    AutoSizeExcelSheet(excelSheet);
                }

                workbook.Write(memory);
                workbook.Close();

                GC.Collect();
                return (memory.ToArray());
            }
        }
        private static byte[] WriteDataTablesToMultipleExcelSheet(ExportOption exportOption, List<DataTable> exportDataTables)
        {
            using (var memory = new MemoryStream())
            {

                var workbook = new XSSFWorkbook();

                var summaryTables = exportDataTables.Where(t=>t.IsSummaryTable()).ToList();
                var mainTables = exportDataTables.Except(summaryTables).ToList();
                if (mainTables.Count > 0)
                {
                    var excelSheet = CreateSheet(workbook, GetSheetName(mainTables));

                    WriteDataTablesToExcelSheet(workbook, excelSheet, mainTables, 0);

                    AutoSizeExcelSheet(excelSheet);

                }
                if (summaryTables.Count > 0)
                {
                    var excelSheet = CreateSheet(workbook, GetSheetName(summaryTables));

                    WriteDataTablesToExcelSheet(workbook, excelSheet, summaryTables, 0);

                    AutoSizeExcelSheet(excelSheet);

                }
                workbook.Write(memory);
                workbook.Close();
                GC.Collect();
                return (memory.ToArray());
            }

        }

   

        private static byte[] WriteDataTablesToSameExcelSheet(ExportOption exportOption, List<DataTable> exportDataTables)
        {
            using (var memory = new MemoryStream())
            {

                var summaryTables = exportDataTables.Where(ExportHelper.IsSummaryTable).ToList();
                var mainTables = exportDataTables.Except(summaryTables).ToList();

                var workbook = new XSSFWorkbook();
                var excelSheet =  CreateSheet(workbook, GetSheetName(mainTables));

                if (exportOption.SummaryPosition == SummaryPosition.Top)
                {
                    var nextIndex = WriteDataTablesToExcelSheet(workbook, excelSheet, summaryTables, 0);
                    excelSheet.CreateRow(nextIndex++);
                    WriteDataTablesToExcelSheet(workbook, excelSheet, mainTables, nextIndex);

                }
                else
                {
                    var nextIndex = WriteDataTablesToExcelSheet(workbook, excelSheet, mainTables, 0);
                    excelSheet.CreateRow(nextIndex++);
                    WriteDataTablesToExcelSheet(workbook, excelSheet, summaryTables, nextIndex);

                }

                AutoSizeExcelSheet(excelSheet);
                workbook.Write(memory);
                workbook.Close();

                GC.Collect();
                return (memory.ToArray());
            }

        }

        private static ISheet CreateSheet(IWorkbook workbook, string sheetName)
        {
            var sheet = string.IsNullOrEmpty(sheetName)
                ? workbook.CreateSheet()
                : workbook.CreateSheet(sheetName);
            return sheet;
        }
        private static void AutoSizeExcelSheet(ISheet excelSheet)
        {
            int numberOfColumns = excelSheet.GetRow(0).PhysicalNumberOfCells;
            for (int i = 0; i <= numberOfColumns; i++)
            {
                excelSheet.AutoSizeColumn(i);
            }
        }

        private static string GetSheetName(List<DataTable> dataTables)
        {
            return string.Join("_", dataTables.Select(t => t.TableName).Where(n => !string.IsNullOrEmpty(n)));
        }
        private static string GetSheetName(DataSet dataSet)
        {
            return dataSet.DataSetName;
        }
        private static int WriteDataTablesToExcelSheet(XSSFWorkbook workbook, ISheet sheet, List<DataTable> dataTables, int startIndex)
        {
            var nextIndex = startIndex;
            foreach (DataTable dataTable in dataTables)
            {
                nextIndex = WriteDataTableToExcelSheet(workbook, sheet, dataTable, nextIndex);
            }

            return nextIndex;
        }
        private static int WriteDataTableToExcelSheet(XSSFWorkbook workbook, ISheet sheet, DataTable dataTable, int startIndex)
        {
            var currentRowIndex = startIndex;
            if (!dataTable.ShouldHideHeader())
            {
                var headerExcelRow = sheet.CreateRow(currentRowIndex);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var column = dataTable.Columns[i];
                    var headerCell = headerExcelRow.CreateCell(i);
                    headerCell.SetCellValue(column.Caption);
                    SetCellFormat(workbook, headerCell, FontBoldWeight.Bold, null);

                }
                currentRowIndex++;
            }

            foreach (DataRow dataTableRow in dataTable.Rows)
            {
                var excelRow = sheet.CreateRow(currentRowIndex);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var cell = excelRow.CreateCell(i);
                    var value = dataTableRow[i];

                    SetCellValue(value, cell, out bool isGroupCell, out string valueFormat);

                    if (isGroupCell)
                    {
                        SetCellFormat(workbook, cell, FontBoldWeight.Bold, valueFormat);
                        var mergedCell = new NPOI.SS.Util.CellRangeAddress(currentRowIndex, currentRowIndex, i, dataTable.Columns.Count - 1);
                        sheet.AddMergedRegion(mergedCell);
                        break;
                    }
                    else
                    {
                        SetCellFormat(workbook, cell, FontBoldWeight.None, valueFormat);
                    }                

                }

                currentRowIndex++;
            }

            return currentRowIndex;

        }

        private static void SetCellFormat(XSSFWorkbook workbook, ICell cell, FontBoldWeight fontBoldWeight, string valueFormat)
        {
            if (fontBoldWeight == FontBoldWeight.None && string.IsNullOrEmpty(valueFormat))
               return;

            var cellStyle = workbook.CreateCellStyle();
            if (!string.IsNullOrEmpty(valueFormat))
            {
                IDataFormat format = workbook.CreateDataFormat();
                short dataFormat = format.GetFormat(valueFormat);
                cellStyle.DataFormat = dataFormat;
            }
            cell.CellStyle = cellStyle;
            if (fontBoldWeight != FontBoldWeight.None)
            {
                var originalFont = cell.CellStyle.GetFont(workbook);
                var font = workbook.CreateFont();
                font.FontName = originalFont.FontName;
                font.FontHeight = originalFont.FontHeight;
                font.Boldweight = (short)fontBoldWeight;
                cell.CellStyle.SetFont(font);
            }
        }
      
        private static void SetCellValue(object cellValue, ICell cell, out bool isGroupCell, out string valueFormat)
        {
            object value;
            isGroupCell = false;
            valueFormat = null;
            if (cellValue is CellInfo cellInfo)
            {
                value = cellInfo.Value;
                isGroupCell = cellInfo.IsGroup;
                valueFormat = cellInfo.ValueFormat;
            }
            else
            {
                value = cellValue;
            }

            if (value is double d)
            {
                cell.SetCellValue(d);
            }
            else if (value.IsNumberValue())
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
            else if (value != null)
            {
                cell.SetCellValue(value.ToString());
            }
            else
            {
                cell.SetCellValue(string.Empty);
            }
        }

        private static bool IsNumberValue(this object value)
        {
            return
                value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal
                || value is sbyte
                || value is byte;
        }
    }
}
