using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microservice.Course.Attributes;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using TableColumn = Microservice.Course.Attributes.TableColumnAttribute;

namespace Microservice.Course.Infrastructure
{
    public static class ExcelHelper
    {
        public static byte[] ExportData<T>(List<T> data, string sheetName = "Sheet1")
            where T : class
        {
            using (var stream = new MemoryStream())
            {
                var (spreadsheetDocument, workbookPart, sheetData) = Initialize(stream, sheetName);

                var dict = AttributeHelper.GetAttribute<T, TableColumn>();

                // Add header row
                var headerRow = new Row();
                dict.Values.ToList().ForEach(p =>
                {
                    headerRow.AppendChild(new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(p.ColumnName)
                    });
                });
                sheetData.AppendChild(headerRow);

                // Add rows
                data.ForEach(dataItem =>
                {
                    var newRow = new Row();
                    dict.Keys.ToList().ForEach(p =>
                    {
                        var cellValue = dataItem.GetPropValue(p).ToString();
                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(cellValue)
                        };
                        newRow.AppendChild(cell);
                    });

                    sheetData.AppendChild(newRow);
                });

                workbookPart.Workbook.Save();
                spreadsheetDocument.Close();
                return stream.ToArray();
            }
        }

        public static byte[] ExportData<T>(List<T> data, List<KeyValuePair<string, Func<T, string>>> colNameToCellValueFnList, string sheetName = "Sheet1")
        {
            using (var stream = new MemoryStream())
            {
                var (spreadsheetDocument, workbookPart, sheetData) = Initialize(stream, sheetName);

                // Add header row
                var headerRow = new Row();
                colNameToCellValueFnList.ForEach(p =>
                {
                    headerRow.AppendChild(new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(p.Key)
                    });
                });
                sheetData.AppendChild(headerRow);

                // Add rows
                data.ForEach(dataItem =>
                {
                    var newRow = new Row();
                    foreach (var colNameToCellValueFn in colNameToCellValueFnList)
                    {
                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(colNameToCellValueFn.Value(dataItem))
                        };
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                });

                workbookPart.Workbook.Save();
                spreadsheetDocument.Close();
                return stream.ToArray();
            }
        }

        public static List<T> ImportData<T>(Stream fileStream, bool isUseHeader = false, bool hasHeaderRow = true)
            where T : class
        {
            try
            {
                var result = new List<T>();

                using var doc = SpreadsheetDocument.Open(fileStream, false);
                var workbookPart = doc.WorkbookPart;
                var sharedStringItems = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ToList();

                var sheetData = workbookPart.WorksheetParts.SelectMany(worksheetPart => worksheetPart.Worksheet?.Elements<SheetData>()).Where(x => x != null);

                foreach (var sheetDatum in sheetData)
                {
                    result.AddRange(GetDataFromSheetData<T>(sheetDatum, sharedStringItems, isUseHeader, hasHeaderRow));
                }

                return result.ToList();
            }
            finally
            {
                fileStream.Close();
            }
        }

        private static List<T> GetDataFromSheetData<T>(SheetData sheetData, List<SharedStringItem> sharedStringItems, bool isUseHeader = false, bool hasHeaderRow = true)
            where T : class
        {
            var data = new List<T>();
            var excelRows = sheetData.Elements<Row>().ToList();

            var excelRowHeadersDict = new Dictionary<string, int>();

            if (hasHeaderRow && isUseHeader)
            {
                excelRowHeadersDict = excelRows.ElementAt(0).Elements<Cell>()
                    .Select(cell => new { Name = GetCellData(sharedStringItems, cell), Index = GetColumnIndex(cell.CellReference) })
                    .Where(x => !string.IsNullOrEmpty(x.Name) && x.Index.HasValue)
                    .ToDictionary(p => p.Name, p => p.Index.GetValueOrDefault());
            }

            // Skip row 0. Row 0 contain the header
            for (var rowIndex = hasHeaderRow ? 1 : 0; rowIndex < excelRows.Count; rowIndex++)
            {
                var rowData = CreateInstanceWithData<T>(excelRows.ElementAt(rowIndex), sharedStringItems, excelRowHeadersDict, hasHeaderRow);
                if (rowData != null)
                {
                    data.Add(rowData);
                }
            }

            return data;
        }

        private static T CreateInstanceWithData<T>(Row row, List<SharedStringItem> sharedStringItems, Dictionary<string, int> excelRowHeadersDict, bool hasHeaderRow)
            where T : class
        {
            var cells = row.Elements<Cell>().ToList();

            if (cells.All(cell => cell.DataType == null || cell.DataType == CellValues.Error))
            {
                return default;
            }

            var cellTotal = cells.Count();
            var o = Activator.CreateInstance<T>();
            var objectProperties = typeof(T).GetProperties();

            foreach (var property in objectProperties)
            {
                var attr = property.GetCustomAttribute<TableColumnAttribute>();
                if (attr == null)
                {
                    continue;
                }

                // if file doesn't have header row, helper will base on column order
                var indexColumn = hasHeaderRow && excelRowHeadersDict.ContainsKey(attr.ColumnName) ? excelRowHeadersDict[attr.ColumnName] : attr.Order;
                if (indexColumn < cellTotal)
                {
                    var cellData = GetCellData(sharedStringItems, cells[indexColumn]);
                    var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                    if (typeConverter.IsValid(cellData))
                    {
                        property.SetValue(o, typeConverter.ConvertFromString(cellData));
                    }
                }
            }

            return o;
        }

        private static string GetCellData(IEnumerable<SharedStringItem> sharedStringItems, Cell cell)
        {
            var cellValue = string.Empty;

            if (cell != null && int.TryParse(cell.InnerText, out var cellId))
            {
                var item = sharedStringItems.ElementAt(cellId);
                if (item.Text != null)
                {
                    cellValue = item.Text.Text;
                }
                else if (item.InnerText != null)
                {
                    cellValue = item.InnerText;
                }
                else if (item.InnerXml != null)
                {
                    cellValue = item.InnerXml;
                }
            }

            return cellValue;
        }

        private static int? GetColumnIndex(string cellReference)
        {
            if (string.IsNullOrEmpty(cellReference))
            {
                return null;
            }

            var columnReference = Regex.Replace(cellReference.ToUpper(), @"[\d]", string.Empty);

            var columnNumber = -1;
            var multiplier = 1;

            foreach (var c in columnReference.ToCharArray().Reverse())
            {
                columnNumber += multiplier * (c - 64);

                multiplier *= 26;
            }

            return columnNumber;
        }

        private static Tuple<SpreadsheetDocument, WorkbookPart, SheetData> Initialize(Stream stream, string sheetName = "Sheet1")
        {
            var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            // Add Sheets to the Workbook.
            var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            var sheet = new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = sheetName
            };
            sheets.Append(sheet);
            return System.Tuple.Create(spreadsheetDocument, workbookPart, sheetData);
        }
    }
}
